using Azure.Storage.Blobs;
using Messenger.Application.Interfaces;
using Messenger.Application.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class MessengerServices
{
	public static IServiceCollection AddMessengerServices(
		this IServiceCollection serviceCollection,
		string messengerBlobContainerName,
		string messengerBlobAccess,
		string messengerBlobUrl)
	{
		serviceCollection.AddPasswordHashServices();
		
		serviceCollection.AddSingleton<IBaseDirService, BaseDirService>(_ => new BaseDirService());

		var blobServiceSettings = new BlobServiceSettings(messengerBlobContainerName, messengerBlobAccess);

		var blobServiceClient = new BlobServiceClient(messengerBlobUrl);
		
		serviceCollection.AddSingleton(_ => blobServiceClient);

		serviceCollection.AddSingleton<IMemoryCache>(_ => new MemoryCache(new MemoryCacheOptions()));

		serviceCollection.AddSingleton<IClaimsService, ClaimsService>();
		
		serviceCollection.AddSingleton<IBlobServiceSettings, BlobServiceSettings>(_ => blobServiceSettings);

		serviceCollection.AddSingleton<IBlobService, BlobService>(_ => new BlobService(blobServiceClient, blobServiceSettings));
		
		return serviceCollection;
	}
}