using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Chats;

public class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, Result<ChatDto>>
{
    private readonly DatabaseContext _context;
    private readonly IBlobService _blobService;
    private readonly IBlobServiceSettings _blobServiceSettings;
	
    public CreateChatCommandHandler(
        DatabaseContext context, 
        IBlobService blobService,
        IBlobServiceSettings blobServiceSettings)
    {
        _context = context;
        _blobService = blobService;
        _blobServiceSettings = blobServiceSettings;
    }
    
    public async Task<Result<ChatDto>> Handle(CreateChatCommand request, CancellationToken cancellationToken)
    {
        var requester = await _context.Users.FirstAsync(u => u.Id == request.RequesterId, cancellationToken);

        var chatByName = await _context.Chats.AnyAsync(c => c.Name == request.Name, cancellationToken);

        if (chatByName)
        {
            return new Result<ChatDto>(new DbEntityExistsError("A chat by that name already exists"));
        }
		
        var newChat = new ChatEntity(
            request.Name,
            request.Title,
            request.Type,
            request.RequesterId,
            avatarFileName: null,
            lastMessageId: null
        );

        if (request.AvatarFile != null)
        {
            var avatarFileName = await _blobService.UploadFileBlobAsync(request.AvatarFile);
            
            newChat.UpdateAvatarFileName(avatarFileName);
        }

        var newChatUser = new ChatUserEntity(
            requester.Id, 
            newChat.Id, 
            canSendMedia: true, 
            muteDateOfExpire: null);
        
        _context.ChatUsers.Add(newChatUser);
        _context.Chats.Add(newChat);
        
        await _context.SaveChangesAsync(cancellationToken);

        var avatarLink = newChat.AvatarFileName != null
            ? $"{_blobServiceSettings.MessengerBlobAccess}/{newChat.AvatarFileName}"
            : null;
        
        var chatDto = new ChatDto
        {
            Id = newChat.Id,
            Name = newChat.Name,
            Title = newChat.Title,
            Type = newChat.Type,
            AvatarLink = avatarLink,
            MembersCount = 1,
            CanSendMedia = true,
            IsOwner = true,
            IsMember = true
        };
        
        return new Result<ChatDto>(chatDto);
    }
}