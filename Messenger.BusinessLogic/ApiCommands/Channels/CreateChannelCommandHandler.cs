using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Channels;

public class CreateChannelCommandHandler : IRequestHandler<CreateChannelCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	
	public CreateChannelCommandHandler(DatabaseContext context, IFileService fileService)
	{
		_context = context;
		_fileService = fileService;
	}
	
	public async Task<Result<ChatDto>> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
	{
		var channelByName = await _context.Chats.FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);
		
		if (channelByName != null) return new Result<ChatDto>(new DbEntityExistsError("A channel by that name already exists"));

		var requestor = await _context.Users.FindAsync(request.RequestorId);

		if (requestor == null) throw new Exception("Requestor not found");
		
		var newChannel = new Chat(
			name: request.Name,
			title: request.Title,
			type: ChatType.Channel,
			ownerId: request.RequestorId,
			avatarLink: null,
			lastMessageId: null
		);

		if (request.AvatarFile != null)
		{
			var fileLink = await _fileService.CreateFileAsync(BaseDirService.GetPathWwwRoot(), request.AvatarFile);

			newChannel.AvatarLink = fileLink;
		}
		
		_context.ChatUsers.Add(new ChatUser {UserId = requestor.Id, ChatId = newChannel.Id});
		_context.Chats.Add(newChannel);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<ChatDto>(
			new ChatDto
			{
				Id = newChannel.Id,	
				Name = newChannel.Name,	
				Title = newChannel.Title,	
				Type = newChannel.Type,	
				MembersCount = newChannel.ChatUsers.Count,	
				CanSendMedia = true,	
				IsOwner = true,
				IsMember = true,	
			});
	}
}
