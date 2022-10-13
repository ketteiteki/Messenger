using Messenger.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.BusinessLogic.DependencyInjection;

public static class DatabaseDependencyInjection
{
    private const string DefaultConnectionString =
        "Server=localhost;User Id=postgres;Password=postgres;Database=MessengerDev;";

    public static IServiceCollection AddDatabaseServices(
        this IServiceCollection serviceCollection,
        string connectionString = DefaultConnectionString)
    {
        var connStr = connectionString ?? DefaultConnectionString;

        serviceCollection.AddDbContext<DatabaseContext>(opt => opt.UseNpgsql(connStr));

        return serviceCollection;
    }
}