using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	
	public CreateConversationCommandHandler(DatabaseContext context, IFileService fileService)
	{
		_context = context;
		_fileService = fileService;
	}
	
	public async Task<Result<ChatDto>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
	{
		var requestor = await _context.Users.FindAsync(request.RequestorId);
			
		if (requestor == null) throw new Exception("Requester not found");
		
		var conversationByName = await _context.Chats
			.FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);

		if (conversationByName != null)
			return new Result<ChatDto>(new DbEntityExistsError("A conference by that name already exists"));

		var newConversation = new Chat(
			name: request.Name,
			ownerId: requestor.Id,
			title: request.Title,
			type: ChatType.Conversation,
			avatarLink: null,
			lastMessageId: null
		);

		if (request.AvatarFile != null)
		{
			var avatarLink = await _fileService.CreateFileAsync(EnvironmentConstants.GetPathWWWRoot(), request.AvatarFile);

			newConversation.AvatarLink = avatarLink;
		}

		newConversation.ChatUsers.Add(new ChatUser {ChatId = newConversation.Id, UserId = requestor.Id});
		
		_context.Chats.Add(newConversation);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<ChatDto>(
			new ChatDto
			{
				Id = newConversation.Id,
				Name = newConversation.Name,
				Title = newConversation.Title,
				Type = newConversation.Type,
				CanSendMedia = true,
				IsOwner = true,
				IsMember = true,
				MembersCount = 1
			});
	}
}