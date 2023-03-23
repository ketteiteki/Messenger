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

		if (requester.AvatarFileName != null)
		{
			await _blobService.DeleteBlobAsync(requester.AvatarFileName);
			
			requester.UpdateAvatarFileName(null);
		}
		
		_context.Users.Remove(requester);
		
		await _context.SaveChangesAsync(cancellationToken);

		var userDto = new UserDto(
			requester.Id,
			requester.DisplayName,
			requester.Nickname,
			requester.Bio,
			avatarLink: null);
		
		return new Result<UserDto>(userDto);
	}
}