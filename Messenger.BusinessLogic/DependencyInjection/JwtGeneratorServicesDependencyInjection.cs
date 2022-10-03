using Messenger.Application.Interfaces;
using Messenger.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.BusinessLogic.DependencyInjection;

public static class JwtGeneratorServicesDependencyInjection
{
	public static IServiceCollection AddJwtGeneratorServices(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddScoped<ITokenService, TokenService>();

		return serviceCollection;
	}
}