using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class ProfileController : ApiControllerBase
{
	public ProfileController(IMediator mediator) : base(mediator) {}
}