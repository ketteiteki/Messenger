using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Infrastructure.DependencyInjection;

public static class AppAuthenticationDependencyInjection
{
    public static IServiceCollection AddAppAuthentication(this IServiceCollection serviceCollection, bool isProduction)
    {
        serviceCollection
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                if (!isProduction) options.Cookie.SameSite = SameSiteMode.None;

                options.Cookie.IsEssential = true;
                options.Cookie.MaxAge = TimeSpan.MaxValue;
            });
        
        return serviceCollection;
    }
}