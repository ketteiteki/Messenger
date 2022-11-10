using MediatR;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
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
public class ConversationsController : ApiControllerBase
{
	public ConversationsController(IMediator mediator) : base(mediator) {}

	
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
	[HttpPost("addUser")]
	public async Task<IActionResult> AddUserToConversation(
		[FromQuery] Guid chatId,
		[FromQuery] Guid userId,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new AddUserToConversationCommand(
			RequesterId: requesterId,
			ChatId: chatId,
			UserId: userId);

		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
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
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
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

	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
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
}