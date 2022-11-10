using MediatR;
using Messenger.BusinessLogic.ApiQueries.Users;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses.Abstractions;
using Messenger.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class UsersController : ApiControllerBase
{
	public UsersController(IMediator mediator) : base(mediator) {}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
	[HttpGet]
	public async Task<IActionResult> GetUserListBySearch(
		[FromQuery] string search,
		CancellationToken cancellationToken, 
		[FromQuery] int limit = 10, 
		[FromQuery] int page = 1)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var query = new GetUserListBySearchQuery(
			RequesterId: requesterId,
			SearchText: search,
			Limit: limit,
			Page: page);
		
		return await RequestAsync(query, cancellationToken);
	}
	
	[ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
	[HttpGet("{userId:guid}")]
	public async Task<IActionResult> GetUser(
		Guid userId,
		CancellationToken cancellationToken)
	{
		var query = new GetUserQuery(
			UserId: userId);
		
		return await RequestAsync(query, cancellationToken);
	}
}