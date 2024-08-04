using MathGame.Infrastructure.Context.Interface.Repositories;
using MathGame.Infrastructure.Repositories;
using MathGame.Infrastructure.Repositories.Interface;
using MathGame.Infrastructure.Repositories.UserMathExpressionRepository;

namespace MathGame.API.Register;

public static class Repositories
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IMathExpressionRepository, MathExpressionRepository>();
        services.AddScoped<IGameSessionRepository, GameSessionRepository>();
        services.AddScoped<IUserInGameSessionRepository, UserInGameSessionRepository>();
        services.AddScoped<IUserMathExpressionRepository, UserMathExpressionRepository>();

        return services;
    }
}
