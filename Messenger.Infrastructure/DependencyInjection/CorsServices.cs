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
                    .WithOrigins("http://localhost:3000", "https://localhost:7400")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }
}