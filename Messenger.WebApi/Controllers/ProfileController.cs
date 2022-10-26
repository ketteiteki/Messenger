using MediatR;
using Messenger.BusinessLogic.ApiCommands.Profiles;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Models.Requests;
using Messenger.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("[controller]")]
[Authorize]
[ApiController]
public class ProfileController : ApiControllerBase
{
	public ProfileController(IMediator mediator) : base(mediator) {}
	
	[ProducesResponseType(typeof(ErrorModel), 409)]
	[ProducesResponseType(typeof(UserDto), 200)]
	[HttpPut]
	public async Task<IActionResult> UpdateProfileData(
		[FromBody] UpdateProfileData request,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var command = new UpdateProfileDataCommand(
			RequesterId: requesterId,
			DisplayName: request.DisplayName,
			NickName: request.NickName,
			Bio: request.Bio);
			
		return await RequestAsync(command, cancellationToken);
	}

	[ProducesResponseType(typeof(UserDto), 200)]
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

	[ProducesResponseType(typeof(UserDto), 200)]
	[HttpDelete("deleteProfile")]
	public async Task<IActionResult> DeleteProfile(CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var query = new DeleteProfileCommand(
			RequesterId: requesterId);
			
		return await RequestAsync(query, cancellationToken);
	}
	
	[ProducesResponseType(typeof(UserDto), 403)]
	[ProducesResponseType(typeof(UserDto), 200)]
	[HttpDelete("deleteProfileAvatar")]
	public async Task<IActionResult> DeleteProfileAvatar(CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var query = new DeleteProfileAvatarCommand(
			RequesterId: requesterId);
			
		return await RequestAsync(query, cancellationToken);
	}
}