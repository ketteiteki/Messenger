using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
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
	private readonly IBaseDirService _baseDirService;

	public UpdateProfileAvatarCommandHandler(
		DatabaseContext context, 
		IFileService fileService,
		IConfiguration configuration, 
		IBaseDirService baseDirService)
	{
		_context = context;
		_fileService = fileService;
		_configuration = configuration;
		_baseDirService = baseDirService;
	}
	
	public async Task<Result<UserDto>> Handle(UpdateProfileAvatarCommand request, CancellationToken cancellationToken)
	{
		var messengerDomainName = _configuration[AppSettingConstants.MessengerDomainName];
		
		var requester = await _context.Users.FirstAsync(u => u.Id == request.RequesterId, cancellationToken);

		if (requester.AvatarLink == null && request.AvatarFile == null)
		{
			return new Result<UserDto>(new ConflictError("Avatar not exists"));
		}

		var pathWwwRoot = _baseDirService.GetPathWwwRoot();

		if (requester.AvatarLink != null && request.AvatarFile == null)
		{
			var avatarFileName = requester.AvatarLink.Split("/")[^1];
			var avatarFilePath = Path.Combine(pathWwwRoot, avatarFileName);
			
			_fileService.DeleteFile(avatarFilePath);
			
			requester.UpdateAvatarLink(null);
			
			_context.Users.Update(requester);
			await _context.SaveChangesAsync(cancellationToken);

			return new Result<UserDto>(new UserDto(requester));
		}

		var avatarLink = await _fileService.CreateFileAsync(pathWwwRoot, request.AvatarFile, messengerDomainName);

		requester.UpdateAvatarLink(avatarLink);
			
		_context.Users.Update(requester);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<UserDto>(new UserDto(requester));

	}
}