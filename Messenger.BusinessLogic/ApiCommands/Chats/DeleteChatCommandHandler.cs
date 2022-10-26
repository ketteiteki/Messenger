using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.BusinessLogic.Services;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Chats;

public class DeleteChatCommandHandler : IRequestHandler<DeleteChatCommand, Result<ChatDto>>
{
    private readonly DatabaseContext _context;
    private readonly IFileService _fileService;
	
    public DeleteChatCommandHandler(DatabaseContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }
	
    public async Task<Result<ChatDto>> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Id == request.ChatId, CancellationToken.None);

        if (chat == null)
            return new Result<ChatDto>(new DbEntityNotFoundError("Chat not found"));
        
        if (chat.Type == ChatType.Dialog)
            return new Result<ChatDto>(new BadRequestError("You may delete only chat of type conversation and channel"));

        if (chat.OwnerId != request.RequesterId)
            return new Result<ChatDto>(new ForbiddenError("It is forbidden to delete someone else's chat"));
		
        if (chat.AvatarLink != null)
            _fileService.DeleteFile(Path.Combine(BaseDirService.GetPathWwwRoot(), chat.AvatarLink.Split("/")[^1]));
		
        _context.Chats.Remove(chat);
        await _context.SaveChangesAsync(cancellationToken);

        return new Result<ChatDto>(
            new ChatDto
            {
                Id = chat.Id,
                Name = chat.Name,
                Title = chat.Title,
                Type = chat.Type
            });
    }
}