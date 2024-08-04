using MathGame.API.Hubs.Services;
using MathGame.Core.Interfaces.Services;
using MathGame.Services.Interface.Services;
using MathGame.Services.Services.GameService;
using MathGame.Services.Services;

namespace MathGame.API.Register;

public static class Services
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<SemaphoreSlim>(new SemaphoreSlim(1, 1));
        services.AddScoped<IMathGeneratorService, MathExpressionGenerator>();
        services.AddScoped<IMathExpressionService, MathExpressionService>();
        services.AddScoped<IUserInGameSessionService, UserInGameSessionService>();
        services.AddScoped<IGameSessionService, GameSessionService>();
        services.AddScoped<IMathGameHubService, MathGameHubService>();
        services.AddScoped<IUserService>(provider =>
        {
            var currentUser = provider.GetRequiredService<IHttpContextAccessor>().HttpContext.User;
            return new UserService(currentUser);
        });

        return services;
    }
}
