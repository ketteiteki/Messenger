using MediatR;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Models.Requests;
using Messenger.BusinessLogic.Responses.Abstractions;
using Messenger.Domain.Constants;
using Messenger.Domain.Enums;
using Messenger.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ChatsController : ControllerBase
{
	private readonly IMediator _mediator;
	
	public ChatsController(IMediator mediator)
	{
		_mediator = mediator;
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
	[HttpGet("{chatId:guid}")]
	public async Task<IActionResult> GetChat(Guid chatId, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var query = new GetChatQuery(requesterId, chatId);

		var result = await _mediator.Send(query, cancellationToken);
		
		return result.ToActionResult();
	}
	
	[ProducesResponseType(typeof(List<ChatDto>), StatusCodes.Status200OK)]
	[HttpGet]
	public async Task<IActionResult> GetChatList(CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var query = new GetChatListQuery(requesterId);

		var result = await _mediator.Send(query, cancellationToken);
		
		return result.ToActionResult();
	}
	
	[ProducesResponseType(typeof(List<ChatDto>), StatusCodes.Status200OK)]
	[HttpGet("search")]
	public async Task<IActionResult> GetChatListBySearch(
		[FromQuery] string searchText,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var query = new GetChatListBySearchQuery(requesterId, searchText);

		var result = await _mediator.Send(query, cancellationToken);
		
		return result.ToActionResult();		
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
	[HttpPost("joinToChat")]
	public async Task<IActionResult> JoinToChat([FromQuery] Guid chatId, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new JoinToChatCommand(requesterId, chatId);

		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
	[HttpPost("leave")]
	public async Task<IActionResult> LeaveFromChat([FromQuery] Guid chatId, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new LeaveFromChatCommand(requesterId, chatId);

		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
	[HttpPost("createChat")]
	public async Task<IActionResult> CreateChat(
		[FromForm] CreateChatRequest request, 
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new CreateChatCommand(
			requesterId, 
			request.Name, 
			request.Title,
			(ChatType)request.Type,
			request.AvatarFile);

		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
	[HttpPut("updateChatData")]
	public async Task<IActionResult> UpdateChatData(
		[FromBody] UpdateChatDataRequest request, 
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new UpdateChatDataCommand(requesterId, request.ChatId, request.Name, request.Title);

		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
	[HttpPut("updateChatAvatar")]
	public async Task<IActionResult> UpdateChatAvatar(
		[FromForm] UpdateChatAvatarRequest request, 
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new UpdateChatAvatarCommand(requesterId, request.ChatId, request.AvatarFile);

		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
	[HttpDelete("deleteChat")]
	public async Task<IActionResult> DeleteChat([FromQuery] Guid chatId, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new DeleteChatCommand(requesterId, chatId);

		var result = await _mediator.Send(command, cancellationToken);

		return result.ToActionResult();
	}
}