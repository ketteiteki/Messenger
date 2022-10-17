using Messenger.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.BusinessLogic.DependencyInjection;

public static class DatabaseDependencyInjection
{
    public static IServiceCollection AddDatabaseServices(
        this IServiceCollection serviceCollection,
        string connectionString)
    {
        serviceCollection.AddDbContext<DatabaseContext>(opt => opt.UseNpgsql(connectionString));

        return serviceCollection;
    }
}