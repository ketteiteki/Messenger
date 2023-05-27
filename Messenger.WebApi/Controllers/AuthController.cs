using MediatR;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.ApiQueries.Auth;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Models.Requests;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.BusinessLogic.Responses.Abstractions;
using Messenger.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ApiControllerBase
{
	public AuthController(IMediator mediator) : base(mediator) {}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(AuthorizationResponse), StatusCodes.Status200OK)]
	[Authorize]
	[HttpGet("authorization")]
	public async Task<IActionResult> Authorization(CancellationToken cancellationToken)
	{
		var requesterGuid = new Guid(HttpContext.User.Claims.First(x => x.Type == ClaimConstants.Id).Value);
		var authorizationToken = HttpContext.Request.Headers
			.First(x => x.Key == HeadersConstants.Authorization).Value.ToString().Split(" ")[^1];
		
		var query = new AuthorizationCommand(requesterGuid, authorizationToken);

		return await RequestAsync(query, cancellationToken);
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
	[Authorize]
	[HttpGet("sessions/{accessToken}")]
	public async Task<IActionResult> GetSession(
		string accessToken,
		CancellationToken cancellationToken)
	{
		var requestId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var query = new GetSessionQuery(
			RequesterId: requestId,
			AccessToken: accessToken);

		return await RequestAsync(query, cancellationToken);
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(List<SessionDto>), StatusCodes.Status200OK)]
	[Authorize]
	[HttpGet("sessions")]
	public async Task<IActionResult> GetSessionList(
		CancellationToken cancellationToken)
	{
		var requestId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var query = new GetSessionListQuery(
			RequesterId: requestId);

		return await RequestAsync(query, cancellationToken);
	}

	[ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(AuthorizationResponse), StatusCodes.Status200OK)]
	[HttpPost("refresh/{refreshToken:guid}")]
	public async Task<IActionResult> Refresh(
		Guid refreshToken,
		[FromHeader(Name = "User-Agent")] string userAgent,
		CancellationToken cancellationToken)
	{
		var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
		
		var command = new RefreshCommand(
			RefreshToken: refreshToken,
			UserAgent: userAgent,
			Ip: ip);
		
		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(AuthorizationResponse), StatusCodes.Status200OK)]
	[HttpPost("registration")]
	public async Task<IActionResult> Registration(
		[FromBody] RegistrationRequest request,
		[FromHeader(Name = "User-Agent")] string userAgent,
		CancellationToken cancellationToken)
	{
		var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
		
		var command = new RegistrationCommand(
			DisplayName: request.DisplayName,
			Nickname: request.Nickname,
			Password: request.Password,
			UserAgent: userAgent,
			Ip: ip);
		
		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(AuthorizationResponse), StatusCodes.Status200OK)]
	[HttpPost("login")]
	public async Task<IActionResult> Login(
		[FromBody] LoginRequest request,
		[FromHeader(Name = "User-Agent")] string userAgent,
		CancellationToken cancellationToken)
	{
		var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
		
		var command = new LoginCommand(
			Nickname: request.Nickname,
			Password: request.Password,
			Ip: ip,
			UserAgent: userAgent);
		
		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
	[HttpDelete("sessions/{sessionId:guid}")]
	public async Task<IActionResult> RemoveSession(
		Guid sessionId,
		CancellationToken cancellationToken)
	{
		var requestId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new RemoveSessionCommand(
			RequesterId: requestId,
			SessionId: sessionId);
		
		return await RequestAsync(command, cancellationToken);
	}
}