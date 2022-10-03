using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.BusinessLogic.DependencyInjection;

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