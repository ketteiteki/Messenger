using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Messanger.Controllers;

[Route("[controller]")]
[ApiController]
public class ProfileController : ApiControllerBase
{
	public ProfileController(IMediator mediator) : base(mediator) {}
}