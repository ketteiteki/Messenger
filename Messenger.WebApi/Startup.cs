using Messenger.BusinessLogic.DependencyInjection;
using Messenger.BusinessLogic.Middlewares;

namespace Messanger;

public class Startup
{
	public void ConfigureServices(IServiceCollection serviceCollection)
	{
		serviceCollection.AddControllers();

		serviceCollection.AddDatabaseServices();
		
		serviceCollection.AddInfrastructureServices();

		serviceCollection.AddMessengerServices();
		
		serviceCollection.AddSwagger();
	}

	public void Configure(IApplicationBuilder applicationBuilder, IHostEnvironment environment)
	{
		if (environment.IsDevelopment())
		{
			applicationBuilder.UseSwagger();
			applicationBuilder.UseSwaggerUI();
		}
		applicationBuilder.UseHttpsRedirection();
		
		applicationBuilder.UseRouting();
		
		applicationBuilder.UseAuthentication();
		applicationBuilder.UseAuthorization();
		
		applicationBuilder.UseMiddleware<ExceptionMiddleware>();
		
		applicationBuilder.UseEndpoints(options =>
		{
			options.MapControllers();
		});
	}
}