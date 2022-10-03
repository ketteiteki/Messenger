using MediatR;
using Messenger.BusinessLogic.Models.RequestResponse;

namespace Messenger.BusinessLogic.Auth.Queries;

public class AuthorizationCommand : IRequest<AuthorizationResponse>
{
	public string AuthorizationToken { get; set; }
}
