using Messenger.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class AppAuthorizationDependencyInjection
{
	public static IServiceCollection AddAppAuthorization(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationResultMiddleware>();
		
		serviceCollection.AddAuthorization();
		
		return serviceCollection;
	}
}