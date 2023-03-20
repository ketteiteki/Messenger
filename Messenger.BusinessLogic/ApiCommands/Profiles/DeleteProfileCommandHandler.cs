using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Profiles;

public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;
	private readonly IBlobService _blobService;

	public DeleteProfileCommandHandler(
		DatabaseContext context,
		IBlobService blobService)
	{
		_context = context;
		_blobService = blobService;
	}
	
	public async Task<Result<UserDto>> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
	{
		var requester = await _context.Users
			.Include(u => u.ChatUsers)
			.FirstAsync(u => u.Id == request.RequesterId, cancellationToken);

		if (requester.AvatarLink != null)
		{
			var avatarFileName = requester.AvatarLink.Split("/")[^1];

			await _blobService.DeleteBlobAsync(avatarFileName);
			
			requester.UpdateAvatarLink(null);
		}
		
		_context.Users.Remove(requester);
		
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<UserDto>(new UserDto(requester));
	}
}