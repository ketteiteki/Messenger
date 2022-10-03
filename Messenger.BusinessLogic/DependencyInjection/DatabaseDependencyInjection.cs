using Messenger.Domain.Constants;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.BusinessLogic.DependencyInjection;

public static class DatabaseDependencyInjection
{
	public static IServiceCollection AddDatabaseServices(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddDbContext<DatabaseContext>(opt =>
			opt.UseNpgsql(EnvironmentConstants.DatabaseConnectionString));

		return serviceCollection;
	}
}