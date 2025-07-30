using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SOAPWebServicesSimple.Middleware;

public class ApplicationEventsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApplicationEventsMiddleware> _logger;

    public ApplicationEventsMiddleware(RequestDelegate next, ILogger<ApplicationEventsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Equivalent to Application_BeginRequest
            _logger.LogInformation("Request started: {Path}", context.Request.Path);
            
            await _next(context);
            
            // After request processing
            _logger.LogInformation("Request completed: {Path}", context.Request.Path);
        }
        catch (Exception ex)
        {
            // Equivalent to Application_Error
            _logger.LogError(ex, "An error occurred processing the request");
            throw;
        }
    }
}

// Extension method to add middleware
public static class ApplicationEventsMiddlewareExtensions
{
    public static IApplicationBuilder UseApplicationEvents(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApplicationEventsMiddleware>();
    }
}