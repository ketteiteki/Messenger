using Messenger.Domain.Constants;
using Messenger.Infrastructure.DependencyInjection;

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

        serviceCollection.AddDatabaseServices(databaseConnectionString);

        serviceCollection.AddInfrastructureServices(signKey);

        serviceCollection.AddMessengerServices();

        serviceCollection.ConfigureCors(CorsPolicyName, allowOrigins);
        
        serviceCollection.AddSwagger();
    }

    public void Configure(IApplicationBuilder applicationBuilder, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            applicationBuilder.UseSwagger();
            applicationBuilder.UseSwaggerUI();
        }
        
        applicationBuilder.UseStaticFiles();

        applicationBuilder.UseHttpsRedirection();

        applicationBuilder.UseRouting();

        applicationBuilder.UseCors(CorsPolicyName);
        
        applicationBuilder.UseAuthentication();
        applicationBuilder.UseAuthorization();

        applicationBuilder.UseEndpoints(options => { options.MapControllers(); });
    }
}