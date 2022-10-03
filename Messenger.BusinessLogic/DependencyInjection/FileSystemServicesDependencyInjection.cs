using Messenger.Application.Interfaces;
using Messenger.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.BusinessLogic.DependencyInjection;

public static class FileSystemServicesDependencyInjection
{
	public static IServiceCollection AddFileSystemServices(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddScoped<IFileService, FileService>();
		
		return serviceCollection;
	}
}