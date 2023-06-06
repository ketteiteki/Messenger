using MediatR;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public record RegistrationCommand(
		string DisplayName,
		string Nickname, 
		string Password)
	: IRequest<Result<AuthorizationResponse>>;