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

namespace Messenger.BusinessLogic.ApiCommands.Channels;

public class CreateChannelCommandHandler : IRequestHandler<CreateChannelCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	private readonly IConfiguration _configuration;
	
	public CreateChannelCommandHandler(DatabaseContext context, IFileService fileService, IConfiguration configuration)
	{
		_context = context;
		_fileService = fileService;
		_configuration = configuration;
	}
	
	public async Task<Result<ChatDto>> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
	{
		var channelByName = await _context.Chats.FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);
		
		if (channelByName != null) return new Result<ChatDto>(new DbEntityExistsError("A channel by that name already exists"));

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

		var r = _configuration[AppSettingConstants.MessengerDomainName];
		
		if (request.AvatarFile != null)
		{
			var fileLink = await _fileService.CreateFileAsync(BaseDirService.GetPathWwwRoot(), request.AvatarFile,
				_configuration[AppSettingConstants.MessengerDomainName]);

			newChannel.AvatarLink = fileLink;
		}
		
		_context.ChatUsers.Add(new ChatUser {UserId = requester.Id, ChatId = newChannel.Id});
		_context.Chats.Add(newChannel);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<ChatDto>(
			new ChatDto
			{
				Id = newChannel.Id,	
				Name = newChannel.Name,	
				Title = newChannel.Title,	
				Type = newChannel.Type,	
				AvatarLink = newChannel.AvatarLink,	
				MembersCount = newChannel.ChatUsers.Count,	
				CanSendMedia = true,	
				IsOwner = true,
				IsMember = true,	
			});
	}
}
