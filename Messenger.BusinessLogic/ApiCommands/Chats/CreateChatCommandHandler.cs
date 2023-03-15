using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.BusinessLogic.Services;
using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
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
        var messengerDomainName = _configuration[AppSettingConstants.MessengerDomainName];
        
        var requester = await _context.Users.FirstAsync(u => u.Id == request.RequesterId, cancellationToken);

        var chatByName = await _context.Chats.AnyAsync(c => c.Name == request.Name, cancellationToken);

        if (chatByName)
        {
            return new Result<ChatDto>(new DbEntityExistsError("A chat by that name already exists"));
        }
		
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
            var pathWwwRoot = BaseDirService.GetPathWwwRoot();
            
            var avatarLink = await _fileService.CreateFileAsync(pathWwwRoot, request.AvatarFile, messengerDomainName);

            newChat.AvatarLink = avatarLink;
        }

        var newChatUser = new ChatUser { UserId = requester.Id, ChatId = newChat.Id };
        
        _context.ChatUsers.Add(newChatUser);
        _context.Chats.Add(newChat);
        
        await _context.SaveChangesAsync(cancellationToken);

        var chatDto = new ChatDto
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
        };
        
        return new Result<ChatDto>(chatDto);
    }
}