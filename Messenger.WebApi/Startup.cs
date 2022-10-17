using Messenger.BusinessLogic.DependencyInjection;
using Messenger.BusinessLogic.Middlewares;
using Messenger.Domain.Constants;

namespace Messenger.WebApi;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers();

        var databaseConnectionString = _configuration.GetConnectionString(AppSettingConstants.DatabaseConnectionString);

        serviceCollection.AddDatabaseServices(databaseConnectionString);

        serviceCollection.AddInfrastructureServices();

        serviceCollection.AddMessengerServices();

        serviceCollection.AddSwagger();
    }

    public void Configure(IApplicationBuilder applicationBuilder, IHostEnvironment environment)
    {
        applicationBuilder.UseSwagger();
        applicationBuilder.UseSwaggerUI();

        applicationBuilder.UseHttpsRedirection();

        applicationBuilder.UseRouting();

        applicationBuilder.UseAuthentication();
        applicationBuilder.UseAuthorization();

        applicationBuilder.UseMiddleware<ExceptionMiddleware>();

        applicationBuilder.UseEndpoints(options => { options.MapControllers(); });
    }
}