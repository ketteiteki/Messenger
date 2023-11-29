using MediatR;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.ApiQueries.Messages;
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
public class MessagesController : ControllerBase
{
	private readonly IMediator _mediator;

	public MessagesController(IMediator mediator)
	{
		_mediator = mediator;
	}

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

		var query = new GetMessageListQuery(requesterId, chatId, limit, fromMessageDateTime);

		var result = await _mediator.Send(query, cancellationToken);
		
		return result.ToActionResult();
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
			requesterId,
			chatId,
			limit,
			fromMessageDateTime,
			searchText);

		var result = await _mediator.Send(query, cancellationToken);
		
		return result.ToActionResult();
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
			requesterId,
			request.Text,
			request.ReplyToId,
			request.ChatId,
			request.Files);

		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
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

		var command = new UpdateMessageCommand(requesterId, request.Id, request.Text);

		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
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

		var command = new DeleteMessageCommand(requesterId, messageId, isDeleteForAll);

		var result = await _mediator.Send(command, cancellationToken);
		
		return result.ToActionResult();
	}
}
