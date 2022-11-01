using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class InfrastructureServices
{
	public static IServiceCollection AddInfrastructureServices(
		this IServiceCollection serviceCollection, 
		string sighKey)
	{
		serviceCollection.AddAppAuthentication(sighKey);

		serviceCollection.AddAppAuthorization();
		
		serviceCollection.AddMediator();

		return serviceCollection;
	}
}