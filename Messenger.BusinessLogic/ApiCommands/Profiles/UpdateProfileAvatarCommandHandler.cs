using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Profiles;

public class UpdateProfileAvatarCommandHandler : IRequestHandler<UpdateProfileAvatarCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;
	private readonly IBlobService _blobService;
	private readonly IBlobServiceSettings _blobServiceSettings;

	public UpdateProfileAvatarCommandHandler(
		DatabaseContext context, 
		IBlobService blobService,
		IBlobServiceSettings blobServiceSettings)
	{
		_context = context;
		_blobService = blobService;
		_blobServiceSettings = blobServiceSettings;
	}
	
	public async Task<Result<UserDto>> Handle(UpdateProfileAvatarCommand request, CancellationToken cancellationToken)
	{
		var requester = await _context.Users.FirstAsync(u => u.Id == request.RequesterId, cancellationToken);

		if (requester.AvatarFileName == null && request.AvatarFile == null)
		{
			return new Result<UserDto>(new ConflictError("Avatar not exists"));
		}

		if (requester.AvatarFileName != null && request.AvatarFile == null)
		{
			await _blobService.DeleteBlobAsync(requester.AvatarFileName);
			
			requester.UpdateAvatarFileName(null);
			
			_context.Users.Update(requester);
			await _context.SaveChangesAsync(cancellationToken);

			var userDtoWithDeletedAvatar = new UserDto(
				requester.Id,
				requester.DisplayName,
				requester.Nickname,
				requester.Bio,
				avatarLink: null);
			
			return new Result<UserDto>(userDtoWithDeletedAvatar);
		}
		
		var avatarFileName = await _blobService.UploadFileBlobAsync(request.AvatarFile);

		requester.UpdateAvatarFileName(avatarFileName);
			
		_context.Users.Update(requester);
		await _context.SaveChangesAsync(cancellationToken);

		var avatarLink = $"{_blobServiceSettings.MessengerBlobAccess}/{avatarFileName}";
		
		var userDto = new UserDto(
			requester.Id,
			requester.DisplayName,
			requester.Nickname,
			requester.Bio,
			avatarLink);
		
		return new Result<UserDto>(userDto);

	}
}