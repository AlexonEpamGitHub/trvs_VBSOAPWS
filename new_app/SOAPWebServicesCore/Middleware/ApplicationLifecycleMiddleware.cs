using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
                // Process SOAP request headers if present
                string soapAction = context.Request.Headers["SOAPAction"].FirstOrDefault() ?? string.Empty;
                if (!string.IsNullOrEmpty(soapAction))
                {
                    _logger.LogInformation("Processing SOAP request with action: {SoapAction}", soapAction);
                    // Record start time for performance tracking
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    
                    await _next(context);
                    
                    // Log performance metrics
                    stopwatch.Stop();
                    _logger.LogInformation("SOAP request processed in {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    // Standard request processing
                    _logger.LogDebug("Processing request for {Path}", context.Request.Path);
                    await _next(context);
                }
            }
            catch (Exception ex)
            {
                // Handle and log SOAP faults specifically
                if (context.Request.ContentType?.Contains("soap") == true)
                {
                    _logger.LogError(ex, "SOAP fault occurred: {Message}", ex.Message);
                }
                else
                {
                    _logger.LogError(ex, "An error occurred during the request: {Message}", ex.Message);
                }
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