using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
    IConfiguration config)
    {
        // Add services to the container.

        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy => policy.AllowAnyHeader()
                .AllowAnyMethod().WithOrigins("https://localhost:4200"));
        });
        //Using Interface for testing
        services.AddScoped<ITokenService, TokenService>();

        return services;

    }
}
