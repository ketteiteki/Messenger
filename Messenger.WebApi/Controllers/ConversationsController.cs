using MediatR;
using Messenger.BusinessLogic.Models.RequestResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConversationsController : ApiControllerBase
{
	public ConversationsController(IMediator mediator) : base(mediator) {}

	[Authorize]
	[HttpGet("{conversationId}")]
	public async Task<IActionResult> GetConversation(Guid conversationId)
	{
		throw new NotImplementedException();
	}
	
	[Authorize]
	[HttpGet]
	public async Task<IActionResult> GetConversations([FromQuery] string search, [FromQuery] int count, [FromQuery] int page)
	{
		throw new NotImplementedException();
	}
	
	[Authorize]
	[HttpPost]
	public async Task<IActionResult> CreateConversation([FromForm] ConversationCreateRequest conversationCreate)
	{
		throw new NotImplementedException();
	}
	
	[Authorize]
	[HttpPost("addUser")]
	public async Task<IActionResult> AddUserToConversation(ConversationAddUserRequest conversationAddUser)
	{
		throw new NotImplementedException();
	}
	
	[Authorize]
	[HttpPost("join")]
	public async Task<IActionResult> JoinToConversation([FromQuery] long conversationId)
	{
		throw new NotImplementedException();
	}
	
	[Authorize]
	[HttpPost("leave")]
	public async Task<IActionResult> LeaveFromConversation([FromQuery] long conversationId)
	{
		throw new NotImplementedException();
	}

	[Authorize]
	[HttpPut]
	public async Task<IActionResult> UpdateConversation([FromForm] ConversationUpdateRequest conversationUpdate)
	{
		throw new NotImplementedException();
	}
	
	[Authorize]
	[HttpDelete("{conversationId}")]
	public async Task<IActionResult> DeleteConversation(long conversationId)
	{
		throw new NotImplementedException();
	}
	
	[Authorize]
	[HttpDelete("removeUser")]
	public async Task<IActionResult> RemoveUserFromConversation(ConversationAddUserRequest conversationAddUser)
	{
		throw new NotImplementedException();
	}
}