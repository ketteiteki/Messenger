using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class SwaggerServices
{
	public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddSwaggerGen();
		
		return serviceCollection;
	}
}