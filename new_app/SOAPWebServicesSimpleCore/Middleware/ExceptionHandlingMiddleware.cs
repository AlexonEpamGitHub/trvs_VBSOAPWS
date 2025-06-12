namespace SOAPWebServicesSimpleCore.Middleware
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using System.Xml;
    using System.Text;

    public class SOAPExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SOAPExceptionHandlingMiddleware> _logger;

        public SOAPExceptionHandlingMiddleware(RequestDelegate next, ILogger<SOAPExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred during request execution");

            // Get environment information
            var env = context.RequestServices.GetService<IWebHostEnvironment>();
            bool isDevelopment = env?.IsDevelopment() == true;
            
            // Set response code and content type
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/xml; charset=utf-8";
            
            // Build SOAP fault message
            var soapFault = new StringBuilder();
            soapFault.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            soapFault.AppendLine("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            soapFault.AppendLine("  <soap:Body>");
            soapFault.AppendLine("    <soap:Fault>");
            soapFault.AppendLine("      <faultcode>soap:Server</faultcode>");
            
            if (isDevelopment)
            {
                // In development mode, include detailed exception information
                soapFault.AppendLine($"      <faultstring>{XmlEscapeString(exception.Message)}</faultstring>");
                soapFault.AppendLine("      <detail>");
                soapFault.AppendLine($"        <ExceptionType>{XmlEscapeString(exception.GetType().FullName)}</ExceptionType>");
                soapFault.AppendLine($"        <StackTrace>{XmlEscapeString(exception.StackTrace)}</StackTrace>");
                
                if (exception.InnerException != null)
                {
                    soapFault.AppendLine($"        <InnerException>{XmlEscapeString(exception.InnerException.Message)}</InnerException>");
                }
                
                soapFault.AppendLine("      </detail>");
            }
            else
            {
                // In production mode, only include generic error message
                soapFault.AppendLine("      <faultstring>An error occurred processing your request. Please contact support.</faultstring>");
            }
            
            soapFault.AppendLine("    </soap:Fault>");
            soapFault.AppendLine("  </soap:Body>");
            soapFault.AppendLine("</soap:Envelope>");
            
            await context.Response.WriteAsync(soapFault.ToString());
        }
        
        private static string XmlEscapeString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            
            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }

    public static class SOAPExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseSOAPExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SOAPExceptionHandlingMiddleware>();
        }
    }
}