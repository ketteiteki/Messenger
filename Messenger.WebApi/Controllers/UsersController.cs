using MediatR;
using Messenger.BusinessLogic.ApiQueries.Users;
using Messenger.BusinessLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class UsersController : ApiControllerBase
{
	public UsersController(IMediator mediator) : base(mediator) {}
	
	[ProducesResponseType(typeof(List<UserDto>), 200)]
	[HttpGet]
	public async Task<IActionResult> GetUserList(
		[FromQuery] string search,
		CancellationToken cancellationToken, 
		[FromQuery] int limit = 10, 
		[FromQuery] int page = 1)
	{
		var query = new GetUserListQuery(
			SearchText: search,
			Limit: limit,
			Page: page);
		
		return await RequestAsync(query, cancellationToken);
	}
	
	[ProducesResponseType(typeof(ErrorModel), 404)]
	[ProducesResponseType(typeof(UserDto), 200)]
	[HttpGet("{userId}")]
	public async Task<IActionResult> GetUser(
		Guid userId,
		CancellationToken cancellationToken)
	{
		var query = new GetUserQuery(
			UserId: userId);
		
		return await RequestAsync(query, cancellationToken);
	}
}