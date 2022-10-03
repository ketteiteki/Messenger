using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.BusinessLogic.Configuration;

public static class MessengerCommandExecutor
{
	internal static async Task<TResult> RequestAsync<TResult>(
		IRequest<TResult> request,
		CancellationToken cancellationToken)
	{
		var scope = MessengerCompositionRoot.CreateScope();
		var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

		return await mediator.Send(request, cancellationToken);
	}
}