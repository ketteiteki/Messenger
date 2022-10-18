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

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	private readonly IConfiguration _configuration;
	
	public CreateConversationCommandHandler(DatabaseContext context, IFileService fileService, IConfiguration configuration)
	{
		_context = context;
		_fileService = fileService;
		_configuration = configuration;
	}
	
	public async Task<Result<ChatDto>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
	{
		var requester = await _context.Users.FindAsync(request.RequesterId);
			
		if (requester == null) throw new Exception("Requester not found");
		
		var conversationByName = await _context.Chats
			.FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);

		if (conversationByName != null)
			return new Result<ChatDto>(new DbEntityExistsError("A conference by that name already exists"));

		var newConversation = new Chat(
			name: request.Name,
			ownerId: requester.Id,
			title: request.Title,
			type: ChatType.Conversation,
			avatarLink: null,
			lastMessageId: null
		);

		if (request.AvatarFile != null)
		{
			var avatarLink = await _fileService.CreateFileAsync(BaseDirService.GetPathWwwRoot(), request.AvatarFile,
				_configuration[AppSettingConstants.MessengerDomainName]);

			newConversation.AvatarLink = avatarLink;
		}

		newConversation.ChatUsers.Add(new ChatUser {ChatId = newConversation.Id, UserId = requester.Id});
		
		_context.Chats.Add(newConversation);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<ChatDto>(
			new ChatDto
			{
				Id = newConversation.Id,
				Name = newConversation.Name,
				Title = newConversation.Title,
				Type = newConversation.Type,
				AvatarLink = newConversation.AvatarLink,
				CanSendMedia = true,
				IsOwner = true,
				IsMember = true,
				MembersCount = 1,
			});
	}
}