using MediatR;
using Messenger.BusinessLogic.ApiCommands.Profiles;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Models.Requests;
using Messenger.BusinessLogic.Responses.Abstractions;
using Messenger.Domain.Constants;
using Messenger.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ProfileController : ControllerBase
{
	private readonly IMediator _mediator;

	public ProfileController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
	[HttpPut("update")]
	public async Task<IActionResult> UpdateProfileData(
		[FromBody] UpdateProfileData request,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var command = new UpdateProfileDataCommand(
			requesterId,
			request.DisplayName,
			request.NickName,
			request.Bio);

		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
	}

	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
	[HttpPut("updateAvatar")]
	public async Task<IActionResult> UpdateProfileAvatar(
		[FromForm] UpdateProfileAvatar request,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var command = new UpdateProfileAvatarCommand(requesterId, request.Avatar);
			
		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
	}

	[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
	[HttpDelete("deleteProfile")]
	public async Task<IActionResult> DeleteProfile(CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var command = new DeleteProfileCommand(requesterId);
			
		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
	}
}