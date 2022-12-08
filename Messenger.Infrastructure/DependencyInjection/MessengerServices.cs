using Messenger.BusinessLogic.Hubs;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class MessengerServices
{
	public static IServiceCollection AddMessengerServices(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddJwtGeneratorServices();

		serviceCollection.AddPasswordHashServices();
		
		serviceCollection.AddFileSystemServices();
		
		return serviceCollection;
	}
}