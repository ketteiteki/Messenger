using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Messenger.BusinessLogic.DependencyInjection;

public static class AppAuthenticationDependencyInjection
{
    public static IServiceCollection AddAppAuthentication(this IServiceCollection serviceCollection,
        string signKey = null)
    {
        var key = signKey ?? "secretAccessTokenKey_1231";

        serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        return serviceCollection;
    }
}