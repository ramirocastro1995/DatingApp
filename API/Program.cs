using API;
using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add controllers to the service collection
builder.Services.AddControllers();

// Add application services to the service collection
// This will typically include services such as repositories, services, etc.
builder.Services.AddApplicationServices(builder.Configuration);

// Add identity services to the service collection
// This will typically include services such as user management, roles, etc.
builder.Services.AddIdentityServices(builder.Configuration);

// Build the application iusng the configured services
var app = builder.Build();

//app.UseHttpsRedirection();


// Add the ExceptionMiddleware to the pipeline. This middleware handles exceptions and provides custom responses.
app.UseMiddleware<ExceptionMiddleware>();

// Configure CORS (Cross-Origin Resource Sharing) to allow any header and any method from the specified origin.
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

// Use the authentication middleware to check if the request has a valid token.
app.UseAuthentication();

// Use the authorization middleware to check if the authenticated user has the right permissions to perform the request.
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(context);
}
catch(Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex,"An error ocurred");
}

app.Run();
