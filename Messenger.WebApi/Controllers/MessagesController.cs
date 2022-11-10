using MediatR;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.ApiQueries.Messages;
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
public class MessagesController : ApiControllerBase
{
	public MessagesController(IMediator mediator) : base(mediator) {}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
	[HttpGet]
	public async Task<IActionResult> GetMessageList(
		[FromQuery] Guid chatId, 
		[FromQuery] DateTime? fromMessageDateTime, 
		[FromQuery] int limit,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var query = new GetMessageListQuery(
			RequesterId: requesterId,
			ChatId: chatId,
			FromMessageDateTime: fromMessageDateTime,
			Limit: limit);
			
		return await RequestAsync(query, cancellationToken);
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
	[HttpGet("search")]
	public async Task<IActionResult> GetMessageListBySearch(
		[FromQuery] Guid chatId, 
		[FromQuery] DateTime? fromMessageDateTime,
		[FromQuery] string searchText,
		[FromQuery] int limit, CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var query = new GetMessageListBySearchQuery(
			RequesterId: requesterId,
			ChatId: chatId,
			FromMessageDateTime: fromMessageDateTime,
			Limit: limit,
			SearchText: searchText);
			
		return await RequestAsync(query, cancellationToken);
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
	[HttpPost("createMessage")]
	public async Task<IActionResult> CreateMessage(
		[FromForm] CreateMessageRequest request,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var command = new CreateMessageCommand(
			RequesterId: requesterId,
			Text: request.Text,
			ChatId: request.ChatId,
			ReplyToId: request.ReplyToId,
			Files: request.Files);
			
		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
	[HttpPut("updateMessage")]
	public async Task<IActionResult> UpdateMessage(
		[FromBody] UpdateMessageRequest request, 
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var command = new UpdateMessageCommand(
			RequesterId: requesterId,
			MessageId: request.Id,
			Text: request.Text);
			
		return await RequestAsync(command, cancellationToken);
	}
	
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
	[HttpDelete("{messageId:guid}")]
	public async Task<IActionResult> DeleteMessage(
		Guid messageId,
		[FromQuery] bool isDeleteForAll,
		CancellationToken cancellationToken)
	{
		var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

		var command = new DeleteMessageCommand(
			RequesterId: requesterId,
			MessageId: messageId,
			IsDeleteForAll: isDeleteForAll);

		return await RequestAsync(command, cancellationToken);
	}
}
