using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;

namespace Messenger.BusinessLogic.ApiCommands.Channels;

public class DeleteChannelCommandHandler : IRequestHandler<DeleteChannelCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;

	public DeleteChannelCommandHandler(DatabaseContext context, IFileService fileService)
	{
		_context = context;
		_fileService = fileService;
	}
	
	public async Task<Result<ChatDto>> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
	{
		var channel = await _context.Chats.FindAsync(request.ChannelId);

		if (channel == null) return new Result<ChatDto>(new DbEntityNotFoundError("Channel not found"));

		if (channel.OwnerId != request.RequesterId) 
			return new Result<ChatDto>(new ForbiddenError("You cannot delete someone else's conference"));

		if (channel.AvatarLink != null)
			_fileService.DeleteFile(Path.Combine("", channel.AvatarLink.Split("/")[^1]));		
		
		_context.Chats.Remove(channel);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<ChatDto>(
			new ChatDto
			{
				Id = channel.Id,
				Name = channel.Name,
				Title = channel.Title,
				Type = channel.Type,
				AvatarLink = channel.AvatarLink,
				MembersCount = channel.ChatUsers.Count, 
				IsOwner = true,
				IsMember = true
			});
	}
}