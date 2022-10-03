using Microsoft.Extensions.DependencyInjection;

namespace Messenger.BusinessLogic.DependencyInjection;

public static class InfrastructureServices
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddAppAuthentication();

		serviceCollection.AddAppAuthorization();
		
		serviceCollection.AddMediator();
		
		return serviceCollection;
	}
}