using Messenger.BusinessLogic;
using Messenger.BusinessLogic.Configuration;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Messenger.IntegrationTests;

public class IntegrationTestBase : IAsyncLifetime
{
	protected DatabaseContext DatabaseContextFixture { get; }
	
	protected MessengerModule MessengerModule { get; set; }
	
	private IServiceProvider ServiceProvider { get; }
	
	protected IntegrationTestBase()
	{
		MessengerStartup.Initialize();

		ServiceProvider = MessengerCompositionRoot.Provider;

		MessengerModule = new MessengerModule();
		
		DatabaseContextFixture = ServiceProvider.GetRequiredService<DatabaseContext>() ??
		                         throw new InvalidOperationException("MangoDbContext service is not registered in the DI.");
	}

	public async Task InitializeAsync()
	{
		await DatabaseContextFixture.Database.MigrateAsync();
		await DatabaseContextFixture.Clear();
	}

	public Task DisposeAsync()
	{
		return Task.CompletedTask;
	}
}