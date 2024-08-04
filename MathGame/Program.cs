using MathGame.API.Entities.User;
using MathGame.API.Hubs;
using MathGame.API.Register;
using MathGame.Services.Interface.Services;
using MathGame.Services.Services;
using Microsoft.AspNetCore.Identity;

namespace MathGame;
public class Program
{
    public static string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.RegisterCors();

        builder.Services.AddSignalR();
         
        builder.Services.AddAuthentication()
                        .AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.RegisterDatabase(builder.Configuration);

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddScoped<ICacheService, CacheService>();

        builder.Services.AddAuthorizationBuilder();

        builder.Services.RegisterRepositories()
                        .RegisterServices()
                        .RegisterAuth();

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
