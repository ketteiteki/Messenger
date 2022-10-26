using MediatR;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Auth;

public record AuthorizationCommand(
	string AuthorizationToken) 
	: IRequest<Result<AuthorizationResponse>>;
