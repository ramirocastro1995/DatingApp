using System.Net;
using System.Text.Json;
using API.Errors;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;
    public ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger,IHostEnvironment env)
    {
        _env = env;
        _logger = logger;
        _next = next;
    }

    //InvokeAsync is needed
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
        catch(Exception ex)
        {
            // Log the error
            _logger.LogError(ex,ex.Message);
            
            // Set the response content type to JSON
            context.Response.ContentType = "application/json";
            // Set the response status code to 500 (Internal Server Error)
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Create a response object, with detailed error info in development environment
            var response = _env.IsDevelopment()
                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                : new ApiException(context.Response.StatusCode, ex.Message, "Internal Server Error");

            // Setup JSON serializer options to use camel case for property names
            var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            
            // Serialize the response object to JSON
            var json = JsonSerializer.Serialize(response,options);

            // Write the JSON response to the HTTP response
            await context.Response.WriteAsync(json);
        }
    }

}
