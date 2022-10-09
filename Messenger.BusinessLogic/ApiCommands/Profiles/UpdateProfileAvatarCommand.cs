using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.ApiCommands.Profiles;

public record UpdateProfileAvatarCommand(
	Guid RequestorId,
	IFormFile? AvatarFile)
	: IRequest<Result<UserDto>>;