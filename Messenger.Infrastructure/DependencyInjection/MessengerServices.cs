using Messenger.Application.Interfaces;
using Messenger.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class MessengerServices
{
	public static IServiceCollection AddMessengerServices(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddJwtGeneratorServices();

		serviceCollection.AddPasswordHashServices();
		
		serviceCollection.AddFileSystemServices();

		serviceCollection.AddSingleton<IBaseDirService, BaseDirService>(_ => new BaseDirService());
		
		return serviceCollection;
	}
}