using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;
using Microsoft.AspNetCore.Hosting;

namespace Messenger.BusinessLogic.Profiles.Commands;

public class UpdateProfileAvatarCommandHandler : IRequestHandler<UpdateProfileAvatarCommand, UserDto>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	private readonly IWebHostEnvironment _webHostEnvironment;

	public UpdateProfileAvatarCommandHandler(DatabaseContext context, IFileService fileService, 
		IWebHostEnvironment webHostEnvironment)
	{
		_context = context;
		_fileService = fileService;
		_webHostEnvironment = webHostEnvironment;
	}
	
	public async Task<UserDto> Handle(UpdateProfileAvatarCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users.FindAsync(request.RequestorId);

		if (user == null) throw new DbEntityNotFoundException("User not found");

		if (user.AvatarLink != null)
		{
			_fileService.DeleteFile(Path.Combine(_webHostEnvironment.WebRootPath, user.AvatarLink.Split("/")[^1]));
			user.AvatarLink = null;
		}

		if (request.AvatarFile != null)
		{
			var avatarLink = await _fileService.CreateFileAsync(_webHostEnvironment.WebRootPath, request.AvatarFile);

			user.AvatarLink = avatarLink;
		}

		_context.Users.Update(user);
		await _context.SaveChangesAsync(cancellationToken);

		return new UserDto(user);
	}
}