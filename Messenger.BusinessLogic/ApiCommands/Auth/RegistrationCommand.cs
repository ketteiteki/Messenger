using MediatR;
using Messenger.BusinessLogic.Models.RequestResponse;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Auth;

public record RegistrationCommand(
		string DisplayName,
		string Nickname, 
		string Password)
	: IRequest<Result<RegistrationResponse>>;