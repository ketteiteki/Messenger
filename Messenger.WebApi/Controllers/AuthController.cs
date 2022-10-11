using MediatR;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.ApiQueries.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Messanger.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ApiControllerBase
{
	public AuthController(IMediator mediator) : base(mediator) {}
	
	[HttpGet("authorization/{token}")]
	public async Task<IActionResult> Authorization(string token, CancellationToken cancellationToken)
	{
		var command = new AuthorizationCommand(token);

		return await RequestAsync(command, cancellationToken);;
	}

	[HttpPost("registration")]
	public async Task<IActionResult> Registration([FromBody] RegistrationCommand registrationCommand,
		CancellationToken cancellationToken)
	{
		return await RequestAsync(registrationCommand, cancellationToken);
	}
	
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginCommand loginCommand, CancellationToken cancellationToken)
	{
		return await RequestAsync(loginCommand, cancellationToken);
	}
}