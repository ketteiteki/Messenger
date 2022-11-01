using MediatR;
using Messenger.Infrastructure.Configuration;

namespace Messenger.Infrastructure;

public class MessengerModule
{
	public async Task<TResult> RequestAsync<TResult>(
		IRequest<TResult> request,
		CancellationToken cancellationToken)
	{ 
		return await MessengerCommandExecutor.RequestAsync(request, cancellationToken);
	}
}