using MediatR;
using Messenger.BusinessLogic.ApiCommands.Dialogs;
using Messenger.BusinessLogic.ApiQueries.Dialogs;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("[controller]")]
[Authorize]
[ApiController]
public class DialogsController : ApiControllerBase
{
    public DialogsController(IMediator mediator) : base(mediator) { }
    
    [ProducesResponseType(typeof(ErrorModel), 404)]
    [ProducesResponseType(typeof(ChatDto), 200)]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetDialog(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
    
        var query = new GetDialogQuery(
            RequesterId: requesterId,
            UserId: userId);
		  
        return await RequestAsync(query, cancellationToken);
    }
    
    [ProducesResponseType(typeof(ErrorModel), 409)]
    [ProducesResponseType(typeof(ErrorModel), 404)]
    [ProducesResponseType(typeof(ChatDto), 200)]
    [HttpPost("createDialog")]
    public async Task<IActionResult> CreateDialog(
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

        var command = new CreateDialogCommand(
            RequesterId: requesterId,
            UserId: userId);
		
        return await RequestAsync(command, cancellationToken);
    }
    
    [ProducesResponseType(typeof(ErrorModel), 404)]
    [ProducesResponseType(typeof(ChatDto), 200)]
    [HttpDelete("deleteDialog")]
    public async Task<IActionResult> DeleteDialog(
        [FromQuery] Guid dialogId, 
        [FromQuery] bool isDeleteForAll,
        CancellationToken cancellationToken)
    {
        var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

        var command = new DeleteDialogCommand(
            RequesterId: requesterId,
            ChatId: dialogId,
            IsDeleteForAll: isDeleteForAll);
		
        return await RequestAsync(command, cancellationToken);
    }
}