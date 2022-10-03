using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;
using Microsoft.AspNetCore.Hosting;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class DeleteConversationCommandHandler : IRequestHandler<DeleteConversationCommand, ChatDto>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	private readonly IWebHostEnvironment _webHostEnvironment;
	
	public DeleteConversationCommandHandler(DatabaseContext context, IFileService fileService, IWebHostEnvironment webHostEnvironment)
	{
		_context = context;
		_fileService = fileService;
		_webHostEnvironment = webHostEnvironment;
	}
	
	public async Task<ChatDto> Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
	{
		var conversation = await _context.Chats.FindAsync(request.ChatId, cancellationToken);
		if (conversation == null) throw new DbEntityNotFoundException("Conversation not found");

		if (conversation.OwnerId != request.RequesterId)
			throw new ForbiddenException("It is forbidden to delete someone else's conversation");
		
		if (conversation.AvatarLink != null)
			_fileService.DeleteFile(Path.Combine(_webHostEnvironment.WebRootPath, conversation.AvatarLink.Split("/")[^1]));
		
		_context.Chats.Remove(conversation);
		await _context.SaveChangesAsync(cancellationToken);

		return new ChatDto
		{
			Id = conversation.Id,
			Name = conversation.Name,
			Title = conversation.Title,
			Type = conversation.Type,
		};
	}
}