using MathGame.Infrastructure.Context.Interface;
using Microsoft.EntityFrameworkCore;

namespace MathGame.API.Register;

public static class Database
{
    public static IServiceCollection RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddDbContext<IMathGameContext, MathGameContext>(options =>
                                options.UseSqlServer(configuration.GetConnectionString("MathGameSqlDatabase")));
    }
}
