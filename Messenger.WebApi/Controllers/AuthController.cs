using MediatR;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.ApiQueries.Auth;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Models.Requests;
using Messenger.BusinessLogic.Models.Responses;
using Messenger.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
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
		var query = new AuthorizationCommand(token);

		return await RequestAsync(query, cancellationToken);
	}
	
	[ProducesResponseType(typeof(ErrorModel), 404)]
	[ProducesResponseType(typeof(SessionDto), 200)]
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
	
	[ProducesResponseType(typeof(ErrorModel), 401)]
	[ProducesResponseType(typeof(List<SessionDto>), 200)]
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

	[ProducesResponseType(typeof(ErrorModel), 401)]
	[ProducesResponseType(typeof(AuthorizationResponse), 200)]
	[HttpPost("refresh/{refreshToken}")]
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
	
	[ProducesResponseType(typeof(ErrorModel), 401)]
	[ProducesResponseType(typeof(AuthorizationResponse), 200)]
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
	
	[ProducesResponseType(typeof(ErrorModel), 401)]
	[ProducesResponseType(typeof(AuthorizationResponse), 200)]
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
	
	[ProducesResponseType(typeof(ErrorModel), 404)]
	[ProducesResponseType(typeof(SessionDto), 200)]
	[HttpDelete("sessions/{sessionId}")]
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