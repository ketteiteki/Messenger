using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class CorsServices
{
    public static IServiceCollection ConfigureCors(
        this IServiceCollection services, string policyName, string allowOrigins)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(policyName, builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(_ => true)
                    .AllowCredentials()
                    .Build();
            });
        });

        return services;
    }
}