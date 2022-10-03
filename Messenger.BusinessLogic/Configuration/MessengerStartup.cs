using Messenger.BusinessLogic.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.BusinessLogic.Configuration;

public static class MessengerStartup
{

	public static void Initialize()
	{
		var serviceCollection = new ServiceCollection();
		
		serviceCollection.AddDatabaseServices();
		
		serviceCollection.AddInfrastructureServices();
	
		serviceCollection.AddMessengerServices();
		
		var provider = serviceCollection.BuildServiceProvider();
		MessengerCompositionRoot.SetProvider(provider);
	}
}