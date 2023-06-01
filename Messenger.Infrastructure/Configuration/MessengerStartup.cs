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
		int cookieExpireTimeSpan,
		string messengerBlobContainerName,
		string messengerBlobAccess,
		string messengerBlobUrl)
	{
		var serviceCollection = new ServiceCollection();

		serviceCollection.AddSingleton<IConfiguration>(configuration);
		
		serviceCollection.AddDatabaseServices(databaseConnectionString);
		
		serviceCollection.AddInfrastructureServices(cookieExpireTimeSpan);
	
		serviceCollection.AddMessengerServices(messengerBlobContainerName, messengerBlobAccess, messengerBlobUrl);

		serviceCollection.AddLogging(builder =>
		{
			builder.AddDebug();
			builder.AddConsole();
		});
		
		var provider = serviceCollection.BuildServiceProvider();
		MessengerCompositionRoot.SetProvider(provider);
	}
}