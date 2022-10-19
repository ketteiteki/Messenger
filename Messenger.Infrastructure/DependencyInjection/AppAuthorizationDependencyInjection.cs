using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class AppAuthorizationDependencyInjection
{
	public static IServiceCollection AddAppAuthorization(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddAuthorization();
		
		return serviceCollection;
	}
}