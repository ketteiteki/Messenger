using Messenger.Application.Interfaces;
using Messenger.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class PasswordHashServiceDependencyInjection
{
	public static IServiceCollection AddPasswordHashServices(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddScoped<IHashService, HashService>();
		
		return serviceCollection;
	}
}