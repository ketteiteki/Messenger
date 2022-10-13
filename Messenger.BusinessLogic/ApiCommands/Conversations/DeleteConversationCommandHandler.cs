using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Constants;
using Messenger.Services;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class DeleteConversationCommandHandler : IRequestHandler<DeleteConversationCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	
	public DeleteConversationCommandHandler(DatabaseContext context, IFileService fileService)
	{
		_context = context;
		_fileService = fileService;
	}
	
	public async Task<Result<ChatDto>> Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
	{
		var conversation = await _context.Chats.FindAsync(request.ChatId);
		if (conversation == null)
			return new Result<ChatDto>(new DbEntityNotFoundError("Conversation not found"));

		if (conversation.OwnerId != request.RequestorId)
			return new Result<ChatDto>(new ForbiddenError("It is forbidden to delete someone else's conversation"));
		
		if (conversation.AvatarLink != null)
			_fileService.DeleteFile(Path.Combine(BaseDirService.GetPathWwwRoot(), conversation.AvatarLink.Split("/")[^1]));
		
		_context.Chats.Remove(conversation);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<ChatDto>(
			new ChatDto
			{
				Id = conversation.Id,
				Name = conversation.Name,
				Title = conversation.Title,
				Type = conversation.Type,
			});
	}
}