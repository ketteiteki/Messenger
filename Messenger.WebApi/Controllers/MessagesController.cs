using MediatR;
using Messenger.BusinessLogic.Models.RequestResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messanger.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
	private readonly IMediator _mediator;

	public MessagesController(IMediator mediator)
	{
		_mediator = mediator;
	}
	
	[Authorize]
	[HttpGet("{messageId}")]
	public async Task<IActionResult> GetMessage(long messageId)
	{
		throw new NotImplementedException();
	}
	
	[Authorize]
	[HttpGet]
	public async Task<IActionResult> GetMessages([FromQuery] long? conversationId, [FromQuery] long? withWhomId, 
		[FromQuery] long? fromMessageId, [FromQuery] int limit = 10)
	{
		throw new NotImplementedException();
	}
	
	[Authorize]
	[HttpGet]
	public async Task<IActionResult> GetMessagesBySearch([FromQuery] long? conversationId, [FromQuery] long? withWhomId,
		[FromQuery] string search, [FromQuery] long? fromMessageId, [FromQuery] int limit = 10)
	{
		throw new NotImplementedException();
	}
	
	[Authorize]
	[HttpPost]
	public async Task<IActionResult> CreateMessage([FromForm] MessageCreateRequest messageCreate)
	{
		throw new NotImplementedException();
	}
	
	[Authorize]
	[HttpPut]
	public async Task<IActionResult> UpdateMessage(MessageUpdateRequest messageUpdate)
	{
		throw new NotImplementedException();
	}
	
	[Authorize]
	[HttpDelete("{messageId}")]
	public async Task<IActionResult> DeleteMessage(long messageId)
	{
		throw new NotImplementedException();
	}
}
