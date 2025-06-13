using Microsoft.AspNetCore.Http;

namespace SOAPWebServicesSimple.Middleware
{
    public class GlobalEventsMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalEventsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // BeginRequest equivalent
                GlobalApplication.OnBeginRequest(context);
                
                // AuthenticateRequest equivalent
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    GlobalApplication.OnAuthenticateRequest(context);
                }
                
                // Execute the rest of the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Error handling equivalent
                GlobalApplication.OnError(ex);
                throw;
            }
        }
    }
    
    // Extension method to add the middleware to IApplicationBuilder
    public static class GlobalEventsMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalEvents(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalEventsMiddleware>();
        }
    }
}