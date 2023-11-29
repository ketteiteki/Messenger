using Messenger.BusinessLogic.Services;
using Messenger.Domain.Constants;
using Messenger.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class HostedServicesDependencyInjection
{
    public static IServiceCollection AddHostedServices(this IServiceCollection serviceCollection)
    {
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var dbContext = serviceProvider.GetService<DatabaseContext>();
        var serviceScopeFactory = serviceProvider.GetService<IServiceScopeFactory>();
        var ticketSerializer = new TicketSerializer();
        var memoryCache = serviceProvider.GetService<IMemoryCache>();
        var configuration = serviceProvider.GetService<IConfiguration>();
        var timeIntervalForCheckingExpiredTickets = 
            configuration.GetValue<int>(AppSettingConstants.TimeIntervalForCheckingExpiredTickets);
        var cookieExpireTimeSpan = configuration.GetValue<int>(AppSettingConstants.CookieExpireTimeSpan);
        
        var ticketStore = new TicketStore(
            serviceScopeFactory,
            ticketSerializer,
            memoryCache,
            cookieExpireTimeSpan);

        serviceCollection.AddHostedService(_ => 
            new RefreshService(dbContext, ticketStore, timeIntervalForCheckingExpiredTickets));
        
        return serviceCollection;
    }
}