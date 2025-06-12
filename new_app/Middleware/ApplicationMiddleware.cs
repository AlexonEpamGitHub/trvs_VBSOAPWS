namespace SOAPWebServicesCore.Middleware
{
    public class ApplicationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApplicationMiddleware> _logger;

        public ApplicationMiddleware(RequestDelegate next, ILogger<ApplicationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Application_BeginRequest equivalent
                _logger.LogDebug("Request beginning: {Path}", context.Request.Path);
                
                await _next(context);
                
                // Application_EndRequest equivalent (if needed)
            }
            catch (Exception ex)
            {
                // Application_Error equivalent
                _logger.LogError(ex, "An unhandled exception occurred");
                throw;
            }
        }
    }

    public static class ApplicationMiddlewareExtensions
    {
        public static IApplicationBuilder UseApplicationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApplicationMiddleware>();
        }
    }
}