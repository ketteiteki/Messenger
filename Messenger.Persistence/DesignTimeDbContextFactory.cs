using Messenger.Application.Services;
using Messenger.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Messenger.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var baseDirService = new BaseDirService();

        var appSettingsPath = baseDirService.GetPathAppSettingsJson(false);
        
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(appSettingsPath)
            .Build();

        var databaseConnectionString =
            configuration[AppSettingConstants.DatabaseConnectionString] ??
            throw new ApplicationException(AppSettingConstants.DatabaseConnectionString);

        var options = new DbContextOptionsBuilder<DatabaseContext>();

        options.UseNpgsql(databaseConnectionString);

        return new DatabaseContext(options.Options);
    }
}
