using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace SOAPWebServicesSimple.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                
                // Only in non-development environments, hide detailed error information
                if (!_env.IsDevelopment())
                {
                    // Return a general error for SOAP requests
                    httpContext.Response.ContentType = "application/xml";
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await httpContext.Response.WriteAsync("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                                                       "<soap:Body>" +
                                                       "<soap:Fault>" +
                                                       "<faultcode>soap:Server</faultcode>" +
                                                       "<faultstring>Internal Server Error</faultstring>" +
                                                       "</soap:Fault>" +
                                                       "</soap:Body>" +
                                                       "</soap:Envelope>");
                }
                else
                {
                    // In development, we can show more details
                    throw;
                }
            }
        }
    }

    // Extension method to add middleware to the pipeline
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}