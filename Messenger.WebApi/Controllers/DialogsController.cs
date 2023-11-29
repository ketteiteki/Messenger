using MediatR;
using Messenger.BusinessLogic.ApiCommands.Dialogs;
using Messenger.BusinessLogic.ApiQueries.Dialogs;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses.Abstractions;
using Messenger.Domain.Constants;
using Messenger.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class DialogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DialogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetDialog(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);
    
        var query = new GetDialogQuery(requesterId, userId);

        var result = await _mediator.Send(query, cancellationToken);
        
        return result.ToActionResult();
    }
    
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
    [HttpPost("createDialog")]
    public async Task<IActionResult> CreateDialog(
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

        var command = new CreateDialogCommand(requesterId, userId);
		
        var result = await _mediator.Send(command, cancellationToken);
        
        return result.ToActionResult();
    }
    
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
    [HttpDelete("deleteDialog")]
    public async Task<IActionResult> DeleteDialog(
        [FromQuery] Guid dialogId, 
        [FromQuery] bool isDeleteForAll,
        CancellationToken cancellationToken)
    {
        var requesterId = new Guid(HttpContext.User.Claims.First(c => c.Type == ClaimConstants.Id).Value);

        var command = new DeleteDialogCommand(requesterId, dialogId, isDeleteForAll);
        
        var result = await _mediator.Send(command, cancellationToken);
        
        return result.ToActionResult();
    }
}