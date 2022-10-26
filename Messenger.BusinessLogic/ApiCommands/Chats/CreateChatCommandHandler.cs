using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.BusinessLogic.Services;
using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Messenger.BusinessLogic.ApiCommands.Chats;

public class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, Result<ChatDto>>
{
    private readonly DatabaseContext _context;
    private readonly IFileService _fileService;
    private readonly IConfiguration _configuration;
	
    public CreateChatCommandHandler(DatabaseContext context, IFileService fileService, IConfiguration configuration)
    {
        _context = context;
        _fileService = fileService;
        _configuration = configuration;
    }
    
    public async Task<Result<ChatDto>> Handle(CreateChatCommand request, CancellationToken cancellationToken)
    {
        if (request.Type == ChatType.Dialog)
            return new Result<ChatDto>(new BadRequestError("Choose chat type between conversation and channel"));
            
        var requester = await _context.Users.FirstAsync(u => u.Id == request.RequesterId, cancellationToken);

        var chatByName = await _context.Chats.FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);
		
        if (chatByName != null) return new Result<ChatDto>(new DbEntityExistsError("A chat by that name already exists"));
		
        var newChat = new Chat(
            name: request.Name,
            title: request.Title,
            type: request.Type,
            ownerId: request.RequesterId,
            avatarLink: null,
            lastMessageId: null
        );

        if (request.AvatarFile != null)
        {
            var avatarLink = await _fileService.CreateFileAsync(BaseDirService.GetPathWwwRoot(), request.AvatarFile,
                _configuration[AppSettingConstants.MessengerDomainName]);

            newChat.AvatarLink = avatarLink;
        }
		
        _context.ChatUsers.Add(new ChatUser {UserId = requester.Id, ChatId = newChat.Id});
        _context.Chats.Add(newChat);
        await _context.SaveChangesAsync(cancellationToken);

        return new Result<ChatDto>(
            new ChatDto
            {
                Id = newChat.Id,	
                Name = newChat.Name,	
                Title = newChat.Title,	
                Type = newChat.Type,	
                AvatarLink = newChat.AvatarLink,	
                MembersCount = 1,	
                CanSendMedia = true,	
                IsOwner = true,
                IsMember = true
            });
    }
}