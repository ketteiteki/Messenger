using Messenger.Application.Interfaces;
using Messenger.Domain.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class AzureBlobInitializer
{
    public static void InitializeAzureBlob(this IApplicationBuilder app, IConfiguration configuration)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

        var blobService = serviceScope.ServiceProvider.GetService<IBlobService>();

        var seedImagesFolder = configuration[AppSettingConstants.SeedImagesFolder];

        blobService.UploadFolderToBlob(seedImagesFolder);
    }
}