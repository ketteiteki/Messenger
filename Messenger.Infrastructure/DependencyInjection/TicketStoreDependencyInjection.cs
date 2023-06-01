using Messenger.BusinessLogic.Services;
using Messenger.Domain.Constants;
using Messenger.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class TicketStoreDependencyInjection
{
    public static IServiceCollection AddTicketStore(this IServiceCollection serviceCollection)
    {
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        var dbContext = serviceProvider.GetService<DatabaseContext>();
        var ticketSerializer = new TicketSerializer();
        var memoryCache = serviceProvider.GetService<IMemoryCache>();
        var configuration = serviceProvider.GetService<IConfiguration>();
        var cookieExpireTimeSpan = configuration.GetValue<int>(AppSettingConstants.CookieExpireTimeSpan);
            
        var ticketStore = new TicketStore(
            dbContext, 
            ticketSerializer, 
            memoryCache, 
            cookieExpireTimeSpan);
        
        serviceCollection.AddSingleton<ITicketStore, TicketStore>(_ => ticketStore);
        
        serviceCollection
            .AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
            .Configure<ITicketStore>((o, _) => o.SessionStore = ticketStore);

        return serviceCollection;
    }
}