using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, ChatDto>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	private readonly IWebHostEnvironment _webHostEnvironment;
	
	public CreateConversationCommandHandler(DatabaseContext context, IFileService fileService, IWebHostEnvironment webHostEnvironment)
	{
		_context = context;
		_fileService = fileService;
		_webHostEnvironment = webHostEnvironment;
	}
	
	public async Task<ChatDto> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
	{
		var requester = await _context.Users.FindAsync(request.RequesterId, cancellationToken);
			
		if (requester == null) throw new Exception("Requester not found");
		
		var conversationByName = await _context.Chats
			.FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);
		
		if (conversationByName != null) 
			throw new DbEntityExistsException("A conference by that name already exists ");

		var newConversation = new Chat(
			name: request.Name,
			ownerId: requester.Id,
			title: request.Title,
			type: ChatType.Сonversation,
			avatarLink: null,
			lastMessageId: null
		);

		if (request.AvatarFile != null)
		{
			var avatarLink = await _fileService.CreateFileAsync(_webHostEnvironment.WebRootPath, request.AvatarFile);

			newConversation.AvatarLink = avatarLink;
		}

		_context.ChatUsers.Add(new ChatUser {ChatId = newConversation.Id, UserId = requester.Id});
		await _context.SaveChangesAsync(cancellationToken);

		return new ChatDto
		{
			Id = newConversation.Id,
			Name = newConversation.Name,
			Title = newConversation.Title,
			Type = newConversation.Type,
			CanSendMedia = true,
			IsOwner = true,
			IsMember = true
		};
	}
}