using MediatR;
using Messenger.BusinessLogic.Models.RequestResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessagesController : ApiControllerBase
{
	public MessagesController(IMediator mediator) : base(mediator) {}
	
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
