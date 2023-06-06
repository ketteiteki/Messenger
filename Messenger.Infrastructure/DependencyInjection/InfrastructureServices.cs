using Messenger.BusinessLogic.Hubs;
using Messenger.BusinessLogic.Hubs.Providers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class InfrastructureServices
{
	public static IServiceCollection AddInfrastructureServices(
		this IServiceCollection serviceCollection, bool isProduction)
	{
		serviceCollection.AddAppAuthentication(isProduction);

		serviceCollection.AddAppAuthorization();
		
		serviceCollection.AddMediator();

		serviceCollection.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

		serviceCollection.AddSignalR().AddHubOptions<ChatHub>(options =>
		{
			options.EnableDetailedErrors = true;
			options.ClientTimeoutInterval = TimeSpan.FromDays(1);
		});

		return serviceCollection;
	}
}