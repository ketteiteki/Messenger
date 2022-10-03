using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Channels.Command;

public class CreateChannelCommandHandler : IRequestHandler<CreateChannelCommand, ChatDto>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	
	public CreateChannelCommandHandler(DatabaseContext context, IFileService fileService)
	{
		_context = context;
		_fileService = fileService;
	}
	
	public async Task<ChatDto> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
	{
		var channelByName = await _context.Chats.FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);

		if (channelByName != null) throw new DbEntityExistsException("A channel by that name already exists");

		var requester = await _context.Users.FindAsync(request.RequesterId);

		if (requester == null) throw new Exception("Requester not found");
		
		var newChannel = new Chat(
			name: request.Name,
			title: request.Title,
			type: ChatType.Channel,
			ownerId: request.RequesterId,
			avatarLink: null,
			lastMessageId: null
		);

		if (request.AvatarFile != null)
		{
			var fileLink = await _fileService.CreateFileAsync("", request.AvatarFile);

			newChannel.AvatarLink = fileLink;
		}
		
		_context.ChatUsers.Add(new ChatUser {UserId = requester.Id, ChatId = newChannel.Id});
		_context.Chats.Add(newChannel);
		await _context.SaveChangesAsync(cancellationToken);

		return new ChatDto
		{
			Id = newChannel.Id,	
			Name = newChannel.Name,	
			Title = newChannel.Title,	
			Type = newChannel.Type,	
			MembersCount = newChannel.ChatUsers.Count,	
			CanSendMedia = true,	
			IsOwner = true,
			IsMember = true,	
		};
	}
}
