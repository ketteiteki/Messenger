using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;

namespace Messenger.BusinessLogic.Channels.Command;

public class DeleteChannelCommandHandler : IRequestHandler<DeleteChannelCommand, ChatDto>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;

	public DeleteChannelCommandHandler(DatabaseContext context, IFileService fileService)
	{
		_context = context;
		_fileService = fileService;
	}
	
	public async Task<ChatDto> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
	{
		var channel = await _context.Chats.FindAsync(request.ChannelId);

		if (channel == null) throw new DbEntityNotFoundException("Channel not found");

		if (channel.OwnerId != request.RequesterId) 
			throw new ForbiddenException("You cannot delete someone else's conference");

		if (channel.AvatarLink != null)
			_fileService.DeleteFile(Path.Combine("", channel.AvatarLink.Split("/")[^1]));		
		
		_context.Chats.Remove(channel);
		await _context.SaveChangesAsync(cancellationToken);

		return new ChatDto
		{
			Id = channel.Id,
			Name = channel.Name,
			Title = channel.Title,
			Type = channel.Type,
			AvatarLink = channel.AvatarLink,
			MembersCount = channel.ChatUsers.Count, 
			IsOwner = true,
			IsMember = true
		};
	}
}