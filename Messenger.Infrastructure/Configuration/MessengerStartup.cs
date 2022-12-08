using Messenger.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Messenger.Infrastructure.Configuration;

public static class MessengerStartup
{

	public static void Initialize(
		IConfigurationRoot configuration,
		string databaseConnectionString,
		string signKey)
	{
		var serviceCollection = new ServiceCollection();

		serviceCollection.AddSingleton<IConfiguration>(configuration);
		
		serviceCollection.AddDatabaseServices(databaseConnectionString);
		
		serviceCollection.AddInfrastructureServices(signKey);
	
		serviceCollection.AddMessengerServices();

		serviceCollection.AddLogging(builder =>
		{
			builder.AddDebug();
			builder.AddConsole();
		});
		
		var provider = serviceCollection.BuildServiceProvider();
		MessengerCompositionRoot.SetProvider(provider);
	}
}