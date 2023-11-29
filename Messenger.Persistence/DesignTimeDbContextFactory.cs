using Messenger.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Messenger.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var databaseConnectionString =
            configuration[AppSettingConstants.DatabaseConnectionString] ??
            throw new ApplicationException(AppSettingConstants.DatabaseConnectionString);

        var options = new DbContextOptionsBuilder<DatabaseContext>();

        options.UseNpgsql(databaseConnectionString);

        return new DatabaseContext(options.Options);
    }
}