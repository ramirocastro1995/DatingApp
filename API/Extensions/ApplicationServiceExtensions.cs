using API.Data;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;

namespace API;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
    IConfiguration config)
    {
        // Add services to the container.

        // Adding a DataContext service to the service collection
        // The DataContext service will use a Sqlite database
        // The connection string for the database is retrieved from the application's configuration
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        
        // Adding a CORS policy to the service collection
        // The policy allows any header, any method, and requests from the origin "https://localhost:4200"
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy => policy.AllowAnyHeader()
                .AllowAnyMethod().WithOrigins("https://localhost:4200"));
        });
        
        // Adding a scoped service of type ITokenService to the service collection
        // The service implementation is TokenService
        // Scoped services are created once per client request
        services.AddScoped<ITokenService, TokenService>();
        
        // Adding a scoped service of type IUserRepository to the service collection
        // The service implementation is UserRepository
        // Scoped services are created once per client request
        services.AddScoped<IUserRepository,UserRepository>();
        
        // Adding AutoMapper to the service collection
        // AutoMapper is a simple little library built to solve a deceptively complex problem - getting rid of code that mapped one object to another
        // The assemblies to search for profiles are retrieved from the current application domain
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

        services.AddScoped<IPhotoService,PhotoService>();

        return services;

    }
}
