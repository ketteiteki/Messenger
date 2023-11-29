using Messenger.BusinessLogic.Hubs;
using Messenger.Domain.Constants;
using Messenger.Infrastructure.DependencyInjection;
using Messenger.Infrastructure.Middlewares;
using Microsoft.OpenApi.Models;

namespace Messenger.WebApi;

public class Startup
{
    private const string CorsPolicyName = "DefaultCors";
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;
    
    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers();

        var databaseConnectionString = _configuration[AppSettingConstants.DatabaseConnectionString];
        var allowOrigins = _configuration[AppSettingConstants.AllowedHosts];
        
        var messengerBlobContainerName = _configuration[AppSettingConstants.BlobContainer];
        var messengerBlobAccess = _configuration[AppSettingConstants.BlobAccess];
        var messengerBlobUrl = _configuration[AppSettingConstants.BlobUrl];

        serviceCollection.AddDatabaseServices(databaseConnectionString);

        serviceCollection.AddInfrastructureServices(_environment.IsDevelopment());

        serviceCollection.AddMessengerServices(messengerBlobContainerName, messengerBlobAccess, messengerBlobUrl);

        serviceCollection.AddTicketStore();

        serviceCollection.AddHostedServices();

        serviceCollection.ConfigureSameSiteNoneCookiePolicy();
        
        serviceCollection.ConfigureCors(CorsPolicyName, allowOrigins);

        serviceCollection.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1",
                new OpenApiInfo { Title = "Messenger API", Version = "v1" });
        });
    }

    public void Configure(IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseSwagger();
        applicationBuilder.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Messenger Api v1");
        });

        applicationBuilder.UseStaticFiles();

        applicationBuilder.UseHttpsRedirection();

        applicationBuilder.UseCookiePolicy();
        
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
        
        applicationBuilder.Map(SpaConstants.Layout, builder => builder.UseSpa(spa => spa.Options.SourcePath = "/wwwroot"));
        applicationBuilder.Map(SpaConstants.ChatInfo, builder => builder.UseSpa(spa => spa.Options.SourcePath = "/wwwroot"));
        applicationBuilder.Map(SpaConstants.CreateChat, builder => builder.UseSpa(spa => spa.Options.SourcePath = "/wwwroot"));
        applicationBuilder.Map(SpaConstants.Login, builder => builder.UseSpa(spa => spa.Options.SourcePath = "/wwwroot"));
        applicationBuilder.Map(SpaConstants.Registration, builder => builder.UseSpa(spa => spa.Options.SourcePath = "/wwwroot"));
        
        applicationBuilder.MigrateDatabase();

        var shouldMigrate = _configuration.GetValue<bool>("ShouldMigrateBlob");
        
        if (shouldMigrate)
        {
            applicationBuilder.InitializeAzureBlob(_configuration);
        }
    }
}