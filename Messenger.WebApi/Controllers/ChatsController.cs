using MediatR;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Models.Requests;
using Messenger.BusinessLogic.Responses.Abstractions;
using Messenger.Domain.Constants;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ChatsController : ApiControllerBase
{
	public ChatsController(IMediator mediator) : base(mediator) {}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
	[HttpGet("{chatId:guid}")]
	public async Task<IActionResult> GetChat(Guid chatId, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var query = new GetChatQuery(
			RequesterId: requesterId,
			ChatId: chatId);

		return await RequestAsync(query, cancellationToken);
	}
	
	[ProducesResponseType(typeof(List<ChatDto>), StatusCodes.Status200OK)]
	[HttpGet]
	public async Task<IActionResult> GetChatList(CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var query = new GetChatListQuery(
			RequesterId: requesterId);

		return await RequestAsync(query, cancellationToken);
	}
	
	[ProducesResponseType(typeof(List<ChatDto>), StatusCodes.Status200OK)]
	[HttpGet("search")]
	public async Task<IActionResult> GetChatListBySearch(
		[FromQuery] string searchText,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var query = new GetChatListBySearchQuery(
			RequesterId: requesterId,
			SearchText: searchText);

		return await RequestAsync(query, cancellationToken);
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
		
		var command = new JoinToChatCommand(
			RequesterId: requesterId,
			ChatId: chatId);

		return await RequestAsync(command, cancellationToken);
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
		
		var command = new LeaveFromChatCommand(
			RequesterId: requesterId,
			ChatId: chatId);

		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
	[HttpPost("createChat")]
	public async Task<IActionResult> CreateChat([FromForm] CreateChatRequest request, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new CreateChatCommand(
			RequesterId: requesterId,
			Name: request.Name,
			Title: request.Title,
			Type: (ChatType)request.Type,
			AvatarFile: request.AvatarFile);

		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
	[HttpPut("updateChatData")]
	public async Task<IActionResult> UpdateChatData([FromBody] UpdateChatDataRequest request, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new UpdateChatDataCommand(
			RequesterId: requesterId,
			ChatId: request.ChatId,
			Name: request.Name,
			Title: request.Title);

		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
	[HttpPut("updateChatAvatar")]
	public async Task<IActionResult> UpdateChatAvatar([FromForm] UpdateChatAvatarRequest request, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new UpdateChatAvatarCommand(
			RequesterId: requesterId,
			ChatId: request.ChatId,
			AvatarFile: request.AvatarFile);

		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
	[HttpDelete("deleteChat")]
	public async Task<IActionResult> DeleteChat([FromQuery] Guid chatId, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new DeleteChatCommand(
			RequesterId: requesterId,
			ChatId: chatId);

		return await RequestAsync(command, cancellationToken);
	}
}