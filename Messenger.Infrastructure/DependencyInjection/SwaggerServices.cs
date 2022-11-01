using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace Messenger.Infrastructure.DependencyInjection;

public static class SwaggerServices
{
	public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddSwaggerGen(options =>
		{
			options.SwaggerDoc("v1", new OpenApiInfo()
			{
				Version = "v1",
				Title = "Messenger Api"
			});
			
			options.AddSecurityDefinition(
				"token",
				new OpenApiSecurityScheme
				{
					Type = SecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = "Bearer",
					In = ParameterLocation.Header,
					Name = HeaderNames.Authorization
				}
			);
			
			options.AddSecurityRequirement(
				new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "token"
							},
						},
						Array.Empty<string>()
					}
				}
			);
		});
		
		return serviceCollection;
	}
}