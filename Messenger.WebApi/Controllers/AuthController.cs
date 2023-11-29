using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.ApiQueries.Auth;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Models.Requests;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses;
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

	[ProducesResponseType(typeof(List<UserSessionDto>), StatusCodes.Status200OK)]
	[Authorize]
	[HttpGet("sessions")]
	public async Task<IActionResult> GetSessions(CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var query = new GetSessionsQuery(requesterId);

		var result = await _mediator.Send(query, cancellationToken);

		return result.ToActionResult();
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
	[ProducesResponseType(typeof(AuthorizationResponse), StatusCodes.Status200OK)]
	[HttpPost("registration")]
	public async Task<IActionResult> Registration(
		[FromBody] RegistrationRequest request,
		CancellationToken cancellationToken)
	{
		var requesterId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimConstants.Id)?.Value;
		var sessionId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimConstants.SessionId)?.Value;
		
		var command = new RegistrationCommand(request.DisplayName, request.Nickname, request.Password);

		var result = await _mediator.Send(command, cancellationToken);

		if (!result.IsSuccess)
		{
			return result.ToActionResult();
		}

		if (sessionId == null || (requesterId != null && result.Value.Id != new Guid(requesterId)))
		{
			var claimsPrincipal = _claimsService.CreateSignInClaims(result.Value.Id);
			var sessionGuid = new Guid(claimsPrincipal.Claims.First(x => x.Type == ClaimConstants.SessionId).Value);

			var authenticationProperties = new AuthenticationProperties
			{
				IsPersistent = true
			};
			
			await HttpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				claimsPrincipal, 
				authenticationProperties);
			
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
		var requesterId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimConstants.Id)?.Value;
		var sessionId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimConstants.SessionId)?.Value;
		
		var command = new LoginCommand(request.Nickname, request.Password);

		var result = await _mediator.Send(command, cancellationToken);

		if (!result.IsSuccess)
		{
			return result.ToActionResult();
		}

		if (sessionId == null || (requesterId != null && result.Value.Id != new Guid(requesterId)))
		{
			var claimsPrincipal = _claimsService.CreateSignInClaims(result.Value.Id);
			var sessionGuid = new Guid(claimsPrincipal.Claims.First(x => x.Type == ClaimConstants.SessionId).Value);

			var authenticationProperties = new AuthenticationProperties
			{
				IsPersistent = true
			};

			await HttpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				claimsPrincipal, 
				authenticationProperties);
			
			result.Value.UpdateCurrentSessionId(sessionGuid);
			return result.ToActionResult();
		} 
		
		result.Value.UpdateCurrentSessionId(new Guid(sessionId));
		
		return result.ToActionResult();
	}
	
	[ProducesResponseType(StatusCodes.Status200OK)]
	[HttpPost("logout")]
	public async Task<IActionResult> Logout()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		return Ok();
	}

	[ProducesResponseType(typeof(DbEntityNotFoundError), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(UserSessionDto), StatusCodes.Status404NotFound)]
	[Authorize]
	[HttpDelete("sessions/{sessionId:guid}")]
	public async Task<IActionResult> TerminateSession(Guid sessionId, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var command = new TerminateSessionCommand(requesterId, sessionId);

		var result = await _mediator.Send(command, cancellationToken);

		return result.ToActionResult();
	}
}