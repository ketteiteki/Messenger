using MediatR;
using Messenger.BusinessLogic.Auth.Commands;
using Messenger.BusinessLogic.Auth.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Messanger.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly IMediator _mediator;

	public AuthController(IMediator mediator)
	{
		_mediator = mediator;
	}
	
	[HttpGet("authorization")]
	public async Task<IActionResult> Authorization()
	{
		if (!HttpContext.Request.Cookies.TryGetValue("authorization", out var result))
			return Unauthorized();

		var command = new AuthorizationCommand
		{
			AuthorizationToken = result!
		};

		var authorizationResult = await _mediator.Send(command);
		
		HttpContext.Response.Cookies.Append("authorization", authorizationResult.AccessToken);
		
		return Ok(authorizationResult);
	}

	[HttpPost("registration")]
	public async Task<IActionResult> Registration([FromBody] RegistrationCommand registrationCommand)
	{
		var registrationResult = await _mediator.Send(registrationCommand);
		
		HttpContext.Response.Cookies.Append("authorization", registrationResult.AccessToken);
		
		return Ok(registrationCommand);
	}
	
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginCommand loginCommand)
	{
		var loginResult = await _mediator.Send(loginCommand);
		
		HttpContext.Response.Cookies.Append("authorization", loginResult.AccessToken);
		
		return Ok(loginResult);
	}
	
	[HttpPost("logout")]
	public IActionResult LogOut()
	{
		HttpContext.Response.Cookies.Delete("authorization");
		
		return Ok();
	}
}