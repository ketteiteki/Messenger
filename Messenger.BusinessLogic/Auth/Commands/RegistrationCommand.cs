using MediatR;
using Messenger.BusinessLogic.Models.RequestResponse;

namespace Messenger.BusinessLogic.Auth.Commands;

public class RegistrationCommand : IRequest<RegistrationResponse>
{
	public string DisplayName { get; set; }
	
	public string Nickname { get; set; }

	public string Password { get; set; }
}