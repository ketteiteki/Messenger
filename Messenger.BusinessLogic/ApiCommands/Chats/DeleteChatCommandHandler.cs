using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Chats;

public class DeleteChatCommandHandler : IRequestHandler<DeleteChatCommand, Result<ChatDto>>
{
    private readonly DatabaseContext _context;
    private readonly IFileService _fileService;
    private readonly IBaseDirService _baseDirService;
	
    public DeleteChatCommandHandler(
        DatabaseContext context, 
        IFileService fileService,
        IBaseDirService baseDirService)
    {
        _context = context;
        _fileService = fileService;
        _baseDirService = baseDirService;
    }
	
    public async Task<Result<ChatDto>> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await _context.Chats
            .Include(c => c.LastMessage)
            .FirstOrDefaultAsync(c => c.Id == request.ChatId, CancellationToken.None);

        if (chat == null)
        {
            return new Result<ChatDto>(new DbEntityNotFoundError("Chat not found"));
        }

        if (chat.Type == ChatType.Dialog)
        {
            return new Result<ChatDto>(new ForbiddenError("You may delete only chat of type conversation and channel"));
        }

        if (chat.OwnerId != request.RequesterId)
        {
            return new Result<ChatDto>(new ForbiddenError("It is forbidden to delete someone else's chat"));
        }

        if (chat.AvatarLink != null)
        {
            var pathWwwRoot = _baseDirService.GetPathWwwRoot();
            var avatarFileName = chat.AvatarLink.Split("/")[^1];

            var avatarFilePath = Path.Combine(pathWwwRoot, avatarFileName);
            
            _fileService.DeleteFile(avatarFilePath);
        }
		
        _context.Chats.Remove(chat);
        
        await _context.SaveChangesAsync(cancellationToken);

        var chatDto = new ChatDto
        {
            Id = chat.Id,
            Name = chat.Name,
            Title = chat.Title,
            Type = chat.Type,
            LastMessageId = chat.LastMessageId,
            LastMessageText = chat.LastMessage?.Text,
            LastMessageAuthorDisplayName =
                chat.LastMessage is { Owner: { } } ? chat.LastMessage.Owner.DisplayName : null,
            LastMessageDateOfCreate = chat.LastMessage?.DateOfCreate,
        };
        
        return new Result<ChatDto>(chatDto);
    }
}