using Messenger.BusinessLogic.Hubs;
using Messenger.Domain.Constants;
using Messenger.Infrastructure.DependencyInjection;
using Messenger.Infrastructure.Middlewares;

namespace Messenger.WebApi;

public class Startup
{
    private const string CorsPolicyName = "DefaultCors";
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers();

        var databaseConnectionString = _configuration[AppSettingConstants.DatabaseConnectionString];
        var signKey = _configuration[AppSettingConstants.MessengerJwtSettingsSecretAccessTokenKey];
        var allowOrigins = _configuration[AppSettingConstants.AllowedHosts];
        
        var messengerBlobContainerName = _configuration[AppSettingConstants.BlobContainer];
        var messengerBlobAccess = _configuration[AppSettingConstants.BlobAccess];
        var messengerBlobUrl = _configuration[AppSettingConstants.BlobUrl];

        serviceCollection.AddDatabaseServices(databaseConnectionString);

        serviceCollection.AddInfrastructureServices(signKey);

        serviceCollection.AddMessengerServices(messengerBlobContainerName, messengerBlobAccess, messengerBlobUrl);
        
        serviceCollection.ConfigureCors(CorsPolicyName, allowOrigins);

        serviceCollection.AddSwagger();
    }

    public void Configure(IApplicationBuilder applicationBuilder, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            applicationBuilder.UseSwagger();
            applicationBuilder.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Messenger Api v1");
                options.RoutePrefix = "";
            });
        }
        
        applicationBuilder.UseStaticFiles();

        applicationBuilder.UseHttpsRedirection();

        applicationBuilder.UseRouting();

        applicationBuilder.UseCors(CorsPolicyName);
        
        applicationBuilder.UseAuthentication();
        applicationBuilder.UseAuthorization();
        
        applicationBuilder.UseMiddleware<ValidationMiddleware>();
        
        applicationBuilder.UseEndpoints(options =>
        {
            options.MapHub<ChatHub>("/notification").RequireCors(CorsPolicyName);
            options.MapControllers();
        });
        
        applicationBuilder.MigrateDatabase();
    }
}