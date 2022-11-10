using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.BusinessLogic.Services;
using Messenger.Domain.Constants;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Messenger.BusinessLogic.ApiCommands.Profiles;

public class UpdateProfileAvatarCommandHandler : IRequestHandler<UpdateProfileAvatarCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	private readonly IConfiguration _configuration;

	public UpdateProfileAvatarCommandHandler(DatabaseContext context, IFileService fileService, IConfiguration configuration)
	{
		_context = context;
		_fileService = fileService;
		_configuration = configuration;
	}
	
	public async Task<Result<UserDto>> Handle(UpdateProfileAvatarCommand request, CancellationToken cancellationToken)
	{
		var requester = await _context.Users.FirstAsync(u => u.Id == request.RequesterId, CancellationToken.None);

		if (requester.AvatarLink == null && request.AvatarFile == null)
		{
			return new Result<UserDto>(new ConflictError("Avatar not exists"));
		}
		
		if (requester.AvatarLink != null && request.AvatarFile == null)
		{
			_fileService.DeleteFile(Path.Combine(BaseDirService.GetPathWwwRoot(), requester.AvatarLink.Split("/")[^1]));
			requester.AvatarLink = null;
			
			_context.Users.Update(requester);
			await _context.SaveChangesAsync(cancellationToken);

			return new Result<UserDto>(new UserDto(requester));
		}

		var avatarLink = await _fileService.CreateFileAsync(BaseDirService.GetPathWwwRoot(), request.AvatarFile,
			_configuration[AppSettingConstants.MessengerDomainName]);

		requester.AvatarLink = avatarLink;
			
		_context.Users.Update(requester);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<UserDto>(new UserDto(requester));

	}
}