using MediatR;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiQueries.Conversations;
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
public class ConversationsController : ControllerBase
{
	private readonly IMediator _mediator;

	public ConversationsController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
	[HttpGet("getUserPermission")]
	public async Task<IActionResult> GetUserPermissions(
		[FromQuery] Guid chatId,
		[FromQuery] Guid userId,
		CancellationToken cancellationToken)
	{
		var query = new GetUserPermissionsQuery(chatId, userId);

		var result = await _mediator.Send(query, cancellationToken);

		return result.ToActionResult();
	} 

	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(RoleUserByChatDto), StatusCodes.Status200OK)]
	[HttpPost("createOrUpdateRoleUser")]
	public async Task<IActionResult> CreateOrUpdateRoleUser(
		[FromBody] CreateOrUpdateRoleUserRequest request,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new CreateOrUpdateRoleUserInConversationCommand(
			requesterId,
			request.ChatId,
			request.UserId,
			request.RoleTitle,
			request.RoleColor,
			request.CanBanUser,
			request.CanChangeChatData,
			request.CanAddAndRemoveUserToConversation,
			request.CanGivePermissionToUser);

		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
	[HttpPost("createPermissions")]
	public async Task<IActionResult> CreatePermissionsUserInConversation(
		[FromBody] CreatePermissionsUserInConversationRequest request,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var command = new CreatePermissionsUserInConversationCommand(
			requesterId,
			request.ChatId,
			request.UserId,
			request.CanSendMedia,
			request.MuteMinutes);

		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
	}
	
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
		
		var command = new AddUserToConversationCommand(requesterId, chatId, userId);

		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
	[HttpPost("banUser")]
	public async Task<IActionResult> BanUser(
		[FromQuery] Guid chatId, 
		[FromQuery] Guid userId, 
		[FromQuery] int banMinutes,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new BanUserInConversationCommand(requesterId, chatId, userId, banMinutes);

		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
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
		
		var command = new UnbanUserInConversationCommand(requesterId, chatId, userId);

		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
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
		
		var command = new RemoveUserFromConversationCommand(requesterId, chatId, userId);

		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
	}

	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(RoleUserByChatDto), StatusCodes.Status200OK)]
	[HttpDelete("removeRoleUser")]
	public async Task<IActionResult> RemoveRoleUserInConversation(
		[FromQuery] Guid chatId,
		[FromQuery] Guid userId,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var command = new RemoveRoleUserInConversationCommand(requesterId, chatId, userId);

		var result = await _mediator.Send(command, cancellationToken);

		return result.ToActionResult();
	}
}