using MediatR;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.ApiQueries.Auth;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ApiControllerBase
{
	public AuthController(IMediator mediator) : base(mediator) {}
	
	[ProducesResponseType(typeof(ErrorModel), 401)]
	[ProducesResponseType(typeof(AuthorizationResponse), 200)]
	[HttpGet("authorization/{token}")]
	public async Task<IActionResult> Authorization(
		string token,
		CancellationToken cancellationToken)
	{
		var command = new AuthorizationCommand(token);

		return await RequestAsync(command, cancellationToken);;
	}

	[ProducesResponseType(typeof(ErrorModel), 401)]
	[ProducesResponseType(typeof(AuthorizationResponse), 200)]
	[HttpPost("registration")]
	public async Task<IActionResult> Registration(
		[FromBody] RegistrationCommand registrationCommand,
		CancellationToken cancellationToken)
	{
		return await RequestAsync(registrationCommand, cancellationToken);
	}
	
	[ProducesResponseType(typeof(ErrorModel), 401)]
	[ProducesResponseType(typeof(AuthorizationResponse), 200)]
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginCommand loginCommand, CancellationToken cancellationToken)
	{
		return await RequestAsync(loginCommand, cancellationToken);
	}
}