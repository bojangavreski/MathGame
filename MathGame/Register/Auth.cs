using MathGame.API.Entities.User;
using Microsoft.AspNetCore.Identity;

namespace MathGame.API.Register;

public static class Auth
{
    public static IServiceCollection RegisterAuth(this IServiceCollection services)
    {
        services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<MathGameContext>()
                .AddApiEndpoints();

        return services;
    }
}
