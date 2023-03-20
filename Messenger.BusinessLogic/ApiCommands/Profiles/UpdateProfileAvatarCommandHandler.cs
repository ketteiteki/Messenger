using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Profiles;

public class UpdateProfileAvatarCommandHandler : IRequestHandler<UpdateProfileAvatarCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;
	private readonly IBlobService _blobService;

	public UpdateProfileAvatarCommandHandler(
		DatabaseContext context, 
		IBlobService blobService)
	{
		_context = context;
		_blobService = blobService;
	}
	
	public async Task<Result<UserDto>> Handle(UpdateProfileAvatarCommand request, CancellationToken cancellationToken)
	{
		var requester = await _context.Users.FirstAsync(u => u.Id == request.RequesterId, cancellationToken);

		if (requester.AvatarLink == null && request.AvatarFile == null)
		{
			return new Result<UserDto>(new ConflictError("Avatar not exists"));
		}

		if (requester.AvatarLink != null && request.AvatarFile == null)
		{
			var avatarFileName = requester.AvatarLink.Split("/")[^1];

			await _blobService.DeleteBlobAsync(avatarFileName);
			
			requester.UpdateAvatarLink(null);
			
			_context.Users.Update(requester);
			await _context.SaveChangesAsync(cancellationToken);

			return new Result<UserDto>(new UserDto(requester));
		}

		var avatarLink = await _blobService.UploadFileBlobAsync(request.AvatarFile);

		requester.UpdateAvatarLink(avatarLink);
			
		_context.Users.Update(requester);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<UserDto>(new UserDto(requester));

	}
}