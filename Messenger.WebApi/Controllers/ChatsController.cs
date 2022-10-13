using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class ChatsController : ApiControllerBase
{
	public ChatsController(IMediator mediator) : base(mediator) {}
}