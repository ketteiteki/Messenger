using MediatR;
using Messenger.BusinessLogic.ApiQueries.Users;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses.Abstractions;
using Messenger.Domain.Constants;
using Messenger.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class UsersController : ControllerBase
{
	private readonly IMediator _mediator;

	public UsersController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
	[HttpGet("search")]
	public async Task<IActionResult> GetUserListBySearch(
		[FromQuery] string search,
		CancellationToken cancellationToken, 
		[FromQuery] int limit = 10, 
		[FromQuery] int page = 1)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var query = new GetUserListBySearchQuery(requesterId, search, limit, page);

		var result = await _mediator.Send(query, cancellationToken);

		return result.ToActionResult();
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
	[HttpGet("chat/{chatId:guid}")]
	public async Task<IActionResult> GetUserListByChat(
		Guid chatId,
		CancellationToken cancellationToken, 
		[FromQuery] int limit = 10, 
		[FromQuery] int page = 1)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var query = new GetUserListByChatQuery(requesterId, chatId, limit, page);
		
		var result = await _mediator.Send(query, cancellationToken);

		return result.ToActionResult();
	}
	
	[ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
	[HttpGet("{userId:guid}")]
	public async Task<IActionResult> GetUser(
		Guid userId,
		CancellationToken cancellationToken)
	{
		var query = new GetUserQuery(userId);
		
		var result = await _mediator.Send(query, cancellationToken);

		return result.ToActionResult();
	}
}