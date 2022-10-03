using MediatR;
using Messenger.BusinessLogic.Auth.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.BusinessLogic.DependencyInjection;

public static class MediatorDependencyInjection
{
	public static IServiceCollection AddMediator(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddMediatR(typeof(RegistrationCommandHandler).Assembly);

		return serviceCollection;
	}
}