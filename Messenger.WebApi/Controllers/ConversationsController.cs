using MediatR;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Models.Requests;
using Messenger.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ConversationsController : ApiControllerBase
{
	public ConversationsController(IMediator mediator) : base(mediator) {}

	[ProducesResponseType(typeof(ErrorModel), 409)]
	[ProducesResponseType(typeof(ErrorModel), 404)]
	[ProducesResponseType(typeof(ErrorModel), 403)]
	[ProducesResponseType(typeof(UserDto), 200)]
	[HttpPost("addUser")]
	public async Task<IActionResult> AddUserToConversation(
		[FromQuery] Guid chatId, [FromQuery] Guid userId, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new AddUserToConversationCommand(
			RequesterId: requesterId,
			ChatId: chatId,
			UserId: userId);

		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(ErrorModel), 404)]
	[ProducesResponseType(typeof(ErrorModel), 403)]
	[ProducesResponseType(typeof(ErrorModel), 400)]
	[ProducesResponseType(typeof(UserDto), 200)]
	[HttpPost("banUser")]
	public async Task<IActionResult> BanUser([FromQuery] Guid chatId, [FromQuery] Guid userId, 
		[FromQuery] DateTime banDateOfExpire, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new BanUserInConversationCommand(
			RequesterId: requesterId,
			ChatId: chatId,
			UserId: userId,
			BanDateOfExpire: banDateOfExpire);

		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(ErrorModel), 404)]
	[ProducesResponseType(typeof(ErrorModel), 403)]
	[ProducesResponseType(typeof(UserDto), 200)]
	[HttpPost("unbanUser")]
	public async Task<IActionResult> UnbanUser(
		[FromQuery] Guid chatId, 
		[FromQuery] Guid userId,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new UnbanUserInConversationCommand(
			RequesterId: requesterId,
			ChatId: chatId,
			UserId: userId);

		return await RequestAsync(command, cancellationToken);
	}

	[ProducesResponseType(typeof(ErrorModel), 404)]
	[ProducesResponseType(typeof(ErrorModel), 403)]
	[ProducesResponseType(typeof(UserDto), 200)]
	[HttpPost("removeUser")]
	public async Task<IActionResult> RemoveUserFromConversation(
		[FromQuery] Guid chatId,
		[FromQuery] Guid userId,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new RemoveUserFromConversationCommand(
			RequesterId: requesterId,
			ChatId: chatId,
			UserId: userId);

		return await RequestAsync(command, cancellationToken);
	}

	[ProducesResponseType(typeof(ErrorModel), 409)]
	[ProducesResponseType(typeof(ErrorModel), 404)]
	[ProducesResponseType(typeof(ErrorModel), 403)]
	[ProducesResponseType(typeof(ChatDto), 200)]
	[HttpPut("updateConversationData")]
	public async Task<IActionResult> UpdateConversationData(
		[FromBody] UpdateConversationDataRequest request,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new UpdateConversationCommand(
			RequesterId: requesterId,
			ChatId: request.ChatId,
			Name: request.Name,
			Title: request.Title);

		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(ErrorModel), 404)]
	[ProducesResponseType(typeof(ErrorModel), 403)]
	[ProducesResponseType(typeof(ChatDto), 200)]
	[HttpPut("updateConversationAvatar")]
	public async Task<IActionResult> UpdateConversationAvatar(
		[FromForm] UpdateConversationAvatarRequest request,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var command = new UpdateConversationAvatarCommand(
			RequesterId: requesterId,
			ChatId: request.ChatId,
			AvatarFile: request.Avatar);
		
		return await RequestAsync(command, cancellationToken);
	}
}