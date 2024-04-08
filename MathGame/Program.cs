using MathGame.API;
using MathGame.API.Entities.User;
using MathGame.API.Hubs;
using MathGame.API.Hubs.Services;
using MathGame.Core.Interfaces.Services;
using MathGame.Infrastructure.Context.Interface;
using MathGame.Infrastructure.Context.Interface.Repositories;
using MathGame.Infrastructure.Repositories;
using MathGame.Infrastructure.Repositories.Interface;
using MathGame.Services.Interface.Services;
using MathGame.Services.Services;
using MathGame.Services.Services.GameService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace MathGame;
public class Program
{
    public static string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                              policy  =>
                              {
                                  policy.WithOrigins("http://localhost:4200")
                                  .AllowAnyHeader()
                                  .AllowAnyMethod()
                                  .AllowCredentials();
                              });   
        });

        builder.Services.AddSignalR();

        // Add services to the container.
        builder.Services.AddAuthentication()
                        .AddBearerToken(IdentityConstants.BearerScheme);
        
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddScoped<ICacheService, CacheService>();

        //Authorization 
        builder.Services.AddAuthorizationBuilder();

        //builder.Services.AddTransient<ClaimsPrincipal>(x => x.GetService<IHttpContextAccessor>().HttpContext.User);

        var connectionString = "Data Source=localhost;Initial Catalog=MathGameDB;Integrated Security=True;Connect Timeout=60;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";


        //Register DbContext
        builder.Services.AddDbContext<IMathGameContext, MathGameContext>(options =>
                                options.UseSqlServer(connectionString));


        //Repositories
        builder.Services.AddScoped<IMathExpressionRepository, MathExpressionRepository>();
        builder.Services.AddScoped<IGameSessionRepository, GameSessionRepository>();
        builder.Services.AddScoped<IUserInGameSessionRepository, UserInGameSessionRepository>();

        //Services
        builder.Services.AddSingleton<SemaphoreSlim>(new SemaphoreSlim(1, 1));
        builder.Services.AddScoped<IMathGeneratorService, MathExpressionGenerator>();
        builder.Services.AddScoped<IMathExpressionService, MathExpressionService>();
        builder.Services.AddScoped<IUserInGameSessionService, UserInGameSessionService>();
        builder.Services.AddScoped<IGameSessionService, GameSessionService>();
        builder.Services.AddScoped<IMathGameHubService, MathGameHubService>();
        builder.Services.AddScoped<IUserService>(provider =>
        {
            var currentUser = provider.GetRequiredService<IHttpContextAccessor>().HttpContext.User;
            return new UserService(currentUser);
        });

        builder.Services.AddIdentityCore<User>()
                        .AddEntityFrameworkStores<MathGameContext>()
                        .AddApiEndpoints();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.MapIdentityApi<User>();
        app.MapHub<MathGameHub>("/answer");

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.UseHttpsRedirection();

        app.UseMiddleware<WebSocketMiddleware>();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseCors(MyAllowSpecificOrigins);
        
        app.MapControllers();

        app.Run();
    }
}
