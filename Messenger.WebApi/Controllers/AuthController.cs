using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.Models.Requests;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses.Abstractions;
using Messenger.Domain.Constants;
using Messenger.WebApi.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly IClaimsService _claimsService;
	
	public AuthController(IMediator mediator, IClaimsService claimsService)
	{
		_mediator = mediator;
		_claimsService = claimsService;
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(AuthorizationResponse), StatusCodes.Status200OK)]
	[Authorize]
	[HttpGet("authorization")]
	public async Task<IActionResult> Authorization(CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		var sessionId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.SessionId).Value);
		
		var query = new AuthorizationCommand(requesterId);
		
		var result = await _mediator.Send(query, cancellationToken);

		if (!result.IsSuccess)
		{
			return result.ToActionResult();
		}
		
		result.Value.UpdateCurrentSessionId(sessionId);
		
		return result.ToActionResult();
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(AuthorizationResponse), StatusCodes.Status200OK)]
	[HttpPost("registration")]
	public async Task<IActionResult> Registration(
		[FromBody] RegistrationRequest request,
		CancellationToken cancellationToken)
	{
		var sessionId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimConstants.SessionId)?.Value;
		
		var command = new RegistrationCommand(request.DisplayName, request.Nickname, request.Password);

		var result = await _mediator.Send(command, cancellationToken);

		if (!result.IsSuccess)
		{
			return result.ToActionResult();
		}

		if (sessionId == null)
		{
			var claimsPrincipal = _claimsService.CreateSignInClaims(result.Value.Id);
			var sessionGuid = new Guid(claimsPrincipal.Claims.First(x => x.Type == ClaimConstants.SessionId).Value);

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
			
			result.Value.UpdateCurrentSessionId(sessionGuid);
			return result.ToActionResult();
		} 
		
		result.Value.UpdateCurrentSessionId(new Guid(sessionId));
		
		return result.ToActionResult();
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(AuthorizationResponse), StatusCodes.Status200OK)]
	[HttpPost("login")]
	public async Task<IActionResult> Login(
		[FromBody] LoginRequest request,
		CancellationToken cancellationToken)
	{
		var sessionId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimConstants.SessionId)?.Value;
		
		var command = new LoginCommand(request.Nickname, request.Password);

		var result = await _mediator.Send(command, cancellationToken);

		if (!result.IsSuccess)
		{
			return result.ToActionResult();
		}

		if (sessionId == null)
		{
			var claimsPrincipal = _claimsService.CreateSignInClaims(result.Value.Id);
			var sessionGuid = new Guid(claimsPrincipal.Claims.First(x => x.Type == ClaimConstants.SessionId).Value);

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
			
			result.Value.UpdateCurrentSessionId(sessionGuid);
			return result.ToActionResult();
		} 
		
		result.Value.UpdateCurrentSessionId(new Guid(sessionId));
		
		return result.ToActionResult();
	}
	
	[ProducesResponseType(typeof(AuthorizationResponse), StatusCodes.Status200OK)]
	[HttpPost("logout")]
	public async Task<IActionResult> Logout()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		return Ok();
	}
}