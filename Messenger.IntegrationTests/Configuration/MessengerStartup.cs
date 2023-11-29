using Messenger.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Messenger.IntegrationTests.Configuration;

public static class MessengerStartup
{
	public static ServiceProvider Initialize(
		IConfigurationRoot configuration,
		string databaseConnectionString,
		string messengerBlobContainerName,
		string messengerBlobAccess,
		string messengerBlobUrl)
	{
		var serviceCollection = new ServiceCollection();

		serviceCollection.AddSingleton<IConfiguration>(configuration);
		
		serviceCollection.AddDatabaseServices(databaseConnectionString);
		
		serviceCollection.AddInfrastructureServices(false);
	
		serviceCollection.AddMessengerServices(messengerBlobContainerName, messengerBlobAccess, messengerBlobUrl);

		serviceCollection.AddLogging(builder =>
		{
			builder.AddDebug();
			builder.AddConsole();
		});
		
		return serviceCollection.BuildServiceProvider();
	}
}