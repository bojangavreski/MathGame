namespace MathGame.API.Register;

public static class Cors
{
    public static string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public static IServiceCollection RegisterCors(this IServiceCollection services)
    {
        return services.AddCors(options =>
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
    }
}
