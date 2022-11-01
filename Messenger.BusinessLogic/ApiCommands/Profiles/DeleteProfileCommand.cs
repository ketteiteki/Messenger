using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Profiles;

public record DeleteProfileCommand(
	Guid RequesterId) 
	: IRequest<Result<UserDto>>;