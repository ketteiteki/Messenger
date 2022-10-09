using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Constants;
using Messenger.Services;

namespace Messenger.BusinessLogic.ApiCommands.Profiles;

public class UpdateProfileAvatarCommandHandler : IRequestHandler<UpdateProfileAvatarCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;

	public UpdateProfileAvatarCommandHandler(DatabaseContext context, IFileService fileService)
	{
		_context = context;
		_fileService = fileService;
	}
	
	public async Task<Result<UserDto>> Handle(UpdateProfileAvatarCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users.FindAsync(request.RequestorId);

		if (user == null) return new Result<UserDto>(new DbEntityNotFoundError("User not found")); 

		if (user.AvatarLink != null)
		{
			_fileService.DeleteFile(Path.Combine(EnvironmentConstants.GetPathWWWRoot(), user.AvatarLink.Split("/")[^1]));
			user.AvatarLink = null;
		}

		if (request.AvatarFile != null)
		{
			var avatarLink = await _fileService.CreateFileAsync(EnvironmentConstants.GetPathWWWRoot(), request.AvatarFile);

			user.AvatarLink = avatarLink;
		}

		_context.Users.Update(user);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<UserDto>(new UserDto(user));
	}
}