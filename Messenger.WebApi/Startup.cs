using Messenger.BusinessLogic.DependencyInjection;
using Messenger.BusinessLogic.Middlewares;

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

        var connectionStr = _configuration.GetConnectionString("DatabaseConnectionString");

        serviceCollection.AddDatabaseServices(connectionStr);

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