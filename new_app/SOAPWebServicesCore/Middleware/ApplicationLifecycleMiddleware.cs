using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SOAPWebServicesCore.Middleware
{
    public class ApplicationLifecycleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApplicationLifecycleMiddleware> _logger;
        
        public ApplicationLifecycleMiddleware(RequestDelegate next, ILogger<ApplicationLifecycleMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Application_BeginRequest equivalent
                _logger.LogDebug("Request started for {Path}", context.Request.Path);
                
                await _next(context);
                
                // End of request handling
            }
            catch (Exception ex)
            {
                // Application_Error equivalent
                _logger.LogError(ex, "An error occurred during the request");
                throw;
            }
        }
    }
    
    // Extension method to register the middleware
    public static class ApplicationLifecycleMiddlewareExtensions
    {
        public static IApplicationBuilder UseApplicationLifecycle(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApplicationLifecycleMiddleware>();
        }
    }
}