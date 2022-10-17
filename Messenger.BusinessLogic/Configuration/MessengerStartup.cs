using Messenger.BusinessLogic.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.BusinessLogic.Configuration;

public static class MessengerStartup
{

	public static void Initialize(
		IConfigurationRoot configuration,
		string databaseConnectionString)
	{
		var serviceCollection = new ServiceCollection();

		serviceCollection.AddSingleton<IConfiguration>(configuration);
		
		serviceCollection.AddDatabaseServices(databaseConnectionString);
		
		serviceCollection.AddInfrastructureServices();
	
		serviceCollection.AddMessengerServices();
		
		var provider = serviceCollection.BuildServiceProvider();
		MessengerCompositionRoot.SetProvider(provider);
	}
}