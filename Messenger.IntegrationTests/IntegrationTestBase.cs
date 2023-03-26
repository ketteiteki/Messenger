using Messenger.Application.Interfaces;
using Messenger.Application.Services;
using Messenger.Domain.Constants;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Configuration;
using Messenger.Persistence;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Messenger.IntegrationTests;

[Collection("Sequential")]
public class IntegrationTestBase : IAsyncLifetime
{
	private DatabaseContext DatabaseContextFixture { get; }
	
	protected MessengerModule MessengerModule { get; }
	
	private IServiceProvider ServiceProvider { get; }

	protected readonly IBaseDirService BaseDirService = new BaseDirService();

	protected readonly IBlobService BlobService;
	
	protected IntegrationTestBase()
	{
		var pathAppSettingsDevelopment = BaseDirService.GetPathAppSettingsJson(isDevelopment: true);

		var configuration = new ConfigurationBuilder()
			.AddJsonFile(pathAppSettingsDevelopment)
			.Build();

		var databaseConnectionString = configuration[AppSettingConstants.DatabaseConnectionStringForIntegrationTests];
		var signKey = configuration[AppSettingConstants.MessengerJwtSettingsSecretAccessTokenKey];
		
		var messengerBlobContainerName = configuration[AppSettingConstants.BlobContainer];
		var messengerBlobAccess = configuration[AppSettingConstants.BlobAccess];
		var messengerBlobUrl = configuration[AppSettingConstants.BlobUrl];

		MessengerStartup.Initialize(
			configuration,
			databaseConnectionString,
			signKey,
			messengerBlobContainerName,
			messengerBlobAccess,
			messengerBlobUrl);

		ServiceProvider = MessengerCompositionRoot.Provider;

		MessengerModule = new MessengerModule();
		
		DatabaseContextFixture = ServiceProvider.GetRequiredService<DatabaseContext>() ??
		                         throw new InvalidOperationException("DatabaseContext service is not registered in the DI.");

		BlobService = ServiceProvider.GetRequiredService<IBlobService>() ??
		              throw new InvalidOperationException("BlobService is not registered in the DI.");;
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