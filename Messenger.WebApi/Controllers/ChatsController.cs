using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Messanger.Controllers;

[Route("[controller]")]
[ApiController]
public class ChatsController : ControllerBase
{
	private readonly IMediator _mediator;

	public ChatsController(IMediator mediator)
	{
		_mediator = mediator;
	}
}