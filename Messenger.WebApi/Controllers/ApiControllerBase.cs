using MediatR;
using Messenger.BusinessLogic.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Messanger.Controllers;

public class ApiControllerBase : ControllerBase
{
	protected readonly IMediator _mediator;

	public ApiControllerBase(IMediator mediator)
	{
		_mediator = mediator;
	}

	protected async Task<IActionResult> RequestAsync<TValue>(
		IRequest<Result<TValue>> request, CancellationToken cancellationToken)
	{
		var result = await _mediator.Send(request, cancellationToken);

		return result.Error switch
		{
			BadRequestError badRequestError => 
				new ObjectResult(new { badRequestError.Message }) { StatusCode = 400 },
			AuthenticationError authenticationError => 
				new ObjectResult(new { authenticationError.Message }) { StatusCode = 403 },
			DbEntityExistsError dbEntityExistsError => 
				new ObjectResult(new { dbEntityExistsError.Message }) { StatusCode = 409 },
			DbEntityNotFoundError dbEntityNotFoundError => 
				new ObjectResult(new { dbEntityNotFoundError.Message }) { StatusCode = 404 },
			ForbiddenError => new ForbidResult(),
			_ => new ObjectResult(result.Value) { StatusCode = 200 }
		};
	}
}