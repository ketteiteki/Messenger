using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class AppAuthenticationDependencyInjection
{
    public static IServiceCollection AddAppAuthentication(this IServiceCollection serviceCollection, int expireTimeSpan)
    {
        serviceCollection
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.IsEssential = true;
                options.Cookie.MaxAge = TimeSpan.MaxValue;
            });
        
        return serviceCollection;
    }
}