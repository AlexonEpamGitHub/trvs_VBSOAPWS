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
    using System.IO;

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
            string correlationId = Guid.NewGuid().ToString();
            _logger.LogError(exception, "Error ID: {CorrelationId} - An unhandled exception occurred during request execution", correlationId);

            // Capture original response body stream
            var originalBodyStream = context.Response.Body;

            // Create a new memory stream
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            // Get environment information
            var env = context.RequestServices.GetService<IWebHostEnvironment>();
            bool isDevelopment = env?.IsDevelopment() == true;
            
            // Set response code and content type for SOAP fault
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/xml; charset=utf-8";
            
            // Build SOAP fault message according to SOAP 1.1/1.2 standards
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
                    soapFault.AppendLine($"        <InnerExceptionType>{XmlEscapeString(exception.InnerException.GetType().FullName)}</InnerExceptionType>");
                }
                
                soapFault.AppendLine("      </detail>");
            }
            else
            {
                // In production mode, only include generic error message with correlation ID for support reference
                soapFault.AppendLine($"      <faultstring>An error occurred processing your request. Please contact support with the following reference: {correlationId}</faultstring>");
                soapFault.AppendLine("      <detail>");
                soapFault.AppendLine($"        <ReferenceId>{correlationId}</ReferenceId>");
                soapFault.AppendLine("      </detail>");
            }
            
            soapFault.AppendLine("    </soap:Fault>");
            soapFault.AppendLine("  </soap:Body>");
            soapFault.AppendLine("</soap:Envelope>");
            
            await context.Response.WriteAsync(soapFault.ToString());
            
            // Copy the contents of the new memory stream to the original stream
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
        
        private static string XmlEscapeString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            
            // Use proper XML escaping for SOAP messages
            return System.Security.SecurityElement.Escape(input) ?? string.Empty;
        }
    }

    public static class SOAPExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseSOAPExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SOAPExceptionHandlingMiddleware>();
        }
        
        // Add extension method for cleaner Program.cs registration
        public static IServiceCollection AddSOAPErrorHandling(this IServiceCollection services)
        {
            // Register any dependencies specific to SOAP error handling
            return services;
        }
    }
}
