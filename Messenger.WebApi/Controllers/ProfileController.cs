using MediatR;
using Messenger.BusinessLogic.ApiCommands.Profiles;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Models.Requests;
using Messenger.BusinessLogic.Responses.Abstractions;
using Messenger.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ProfileController : ApiControllerBase
{
	public ProfileController(IMediator mediator) : base(mediator) {}
	
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
			RequesterId: requesterId,
			DisplayName: request.DisplayName,
			Nickname: request.NickName,
			Bio: request.Bio);
			
		return await RequestAsync(command, cancellationToken);
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

		var command = new UpdateProfileAvatarCommand(
			RequesterId: requesterId,
			AvatarFile: request.Avatar);
			
		return await RequestAsync(command, cancellationToken);
	}

	[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
	[HttpDelete("deleteProfile")]
	public async Task<IActionResult> DeleteProfile(CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var command = new DeleteProfileCommand(
			RequesterId: requesterId);
			
		return await RequestAsync(command, cancellationToken);
	}
}