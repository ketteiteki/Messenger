using MediatR;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public record LoginCommand(
		string NickName,
		string Password) 
	: IRequest<Result<AuthorizationResponse>>;