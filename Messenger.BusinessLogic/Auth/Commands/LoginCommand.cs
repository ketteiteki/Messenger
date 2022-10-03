using MediatR;
using Messenger.BusinessLogic.Models.RequestResponse;

namespace Messenger.BusinessLogic.Auth.Commands;

public class LoginCommand : IRequest<LoginResponse>
{
	public string NickName { get; set; } = null!;
	
	public string Password { get; set; } = null!;
}