using Messenger.BusinessLogic.Hubs;
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

		serviceCollection.AddSignalR().AddHubOptions<ChatHub>(options =>
		{
			options.EnableDetailedErrors = true;
			options.ClientTimeoutInterval = TimeSpan.FromDays(1);
		});

		return serviceCollection;
	}
}