using MediatR;
using Messenger.BusinessLogic.Configuration;

namespace Messenger.BusinessLogic;

public class MessengerModule
{
	public async Task<TResult> RequestAsync<TResult>(
		IRequest<TResult> request,
		CancellationToken cancellationToken)
	{ 
		return await MessengerCommandExecutor.RequestAsync(request, cancellationToken);
	}
}