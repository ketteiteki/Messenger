using MediatR;
using Messenger.Application.Interfaces;
using Messenger.Application.Services;
using Messenger.Domain.Constants;
using Messenger.IntegrationTests.Configuration;
using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Messenger.IntegrationTests;

[Collection("Sequential")]
public class IntegrationTestBase : IAsyncLifetime
{
	protected DatabaseContext DatabaseContextFixture { get; }
	
	private IServiceProvider ServiceProvider { get; }

	protected readonly IBaseDirService BaseDirService = new BaseDirService();

	protected readonly IBlobService BlobService;
	
	protected IntegrationTestBase()
	{
		var pathAppSettings = BaseDirService.GetPathAppSettingsJson(false);

		var configuration = new ConfigurationBuilder()
			.AddJsonFile(pathAppSettings)
			.Build();

		var databaseConnectionString = configuration[AppSettingConstants.DatabaseConnectionStringForIntegrationTests];
		
		var messengerBlobContainerName = configuration[AppSettingConstants.BlobContainer];
		var messengerBlobAccess = configuration[AppSettingConstants.BlobAccess];
		var messengerBlobUrl = configuration[AppSettingConstants.BlobUrl];

		var serviceProvider = MessengerStartup.Initialize(
			configuration,
			databaseConnectionString,
			messengerBlobContainerName,
			messengerBlobAccess,
			messengerBlobUrl);

		ServiceProvider = serviceProvider;
		
		DatabaseContextFixture = ServiceProvider.GetRequiredService<DatabaseContext>() ??
		                         throw new InvalidOperationException("DatabaseContext service is not registered in the DI.");

		BlobService = ServiceProvider.GetRequiredService<IBlobService>() ??
		              throw new InvalidOperationException("BlobService is not registered in the DI.");
	}

	protected async Task<TResult> RequestAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken)
	{
		if (ServiceProvider == null)
		{
			throw new Exception("ServiceProvider is null");
		}
		
		var mediator = ServiceProvider.GetRequiredService<IMediator>();

		return await mediator.Send(request, cancellationToken);
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