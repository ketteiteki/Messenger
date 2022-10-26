using MediatR;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Models.Requests;
using Messenger.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("[controller]")]
[Authorize]
[ApiController]
public class ChatsController : ApiControllerBase
{
	public ChatsController(IMediator mediator) : base(mediator) {}
	
	[ProducesResponseType(typeof(ErrorModel), 404)]
	[ProducesResponseType(typeof(ChatDto), 200)]
	[HttpGet("{chatId}")]
	public async Task<IActionResult> GetChat(Guid chatId, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var query = new GetChatQuery(
			RequesterId: requesterId,
			ChatId: chatId);

		return await RequestAsync(query, cancellationToken);
	}
	
	[ProducesResponseType(typeof(List<ChatDto>), 200)]
	[HttpGet]
	public async Task<IActionResult> GetChatList(CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var query = new GetChatListQuery(
			RequesterId: requesterId);

		return await RequestAsync(query, cancellationToken);
	}
	
	[ProducesResponseType(typeof(ErrorModel), 409)]
	[ProducesResponseType(typeof(ErrorModel), 404)]
	[ProducesResponseType(typeof(ErrorModel), 403)]
	[ProducesResponseType(typeof(List<ChatDto>), 200)]
	[HttpPost("joinToChat")]
	public async Task<IActionResult> JoinToChat([FromQuery] Guid chatId, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new JoinToChatCommand(
			RequesterId: requesterId,
			ChatId: chatId);

		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(ErrorModel), 409)]
	[ProducesResponseType(typeof(ErrorModel), 404)]
	[ProducesResponseType(typeof(ErrorModel), 403)]
	[ProducesResponseType(typeof(List<ChatDto>), 200)]
	[HttpPost("leave")]
	public async Task<IActionResult> LeaveFromChat([FromQuery] Guid chatId, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new LeaveFromChatCommand(
			RequesterId: requesterId,
			ChatId: chatId);

		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(ErrorModel), 409)]
	[ProducesResponseType(typeof(ErrorModel), 400)]
	[ProducesResponseType(typeof(ChatDto), 200)]
	[HttpPost("createChat")]
	public async Task<IActionResult> CreateChat([FromForm] CreateChatRequest request, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
		
		var command = new CreateChatCommand(
			RequesterId: requesterId,
			Name: request.Name,
			Title: request.Title,
			Type: request.Type,
			AvatarFile: request.AvatarFile);

		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(ErrorModel), 404)]
	[ProducesResponseType(typeof(ErrorModel), 401)]
	[ProducesResponseType(typeof(ErrorModel), 400)]
	[ProducesResponseType(typeof(ChatDto), 200)]
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