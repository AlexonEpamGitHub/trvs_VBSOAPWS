using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SoapCore;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml;

namespace SOAPWebServicesCore.Middleware
{
    public class SoapExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SoapExceptionMiddleware> _logger;
        
        public SoapExceptionMiddleware(RequestDelegate next, ILogger<SoapExceptionMiddleware> logger)
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
                if (context.Request.ContentType?.Contains("soap") == true)
                {
                    _logger.LogError(ex, "SOAP fault occurred: {Message}", ex.Message);
                    
                    // Convert to SOAP fault
                    var fault = new FaultException(ex.Message);
                    
                    // Set appropriate status code
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "text/xml; charset=utf-8";
                    
                    // Write SOAP fault response
                    await WriteSOAPFaultResponseAsync(context, fault);
                }
                else
                {
                    // Let the standard exception middleware handle it
                    throw;
                }
            }
        }
        
        private async Task WriteSOAPFaultResponseAsync(HttpContext context, FaultException fault)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                OmitXmlDeclaration = false,
                Indent = true
            };
            
            using (XmlWriter writer = XmlWriter.Create(context.Response.Body, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("soap", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
                writer.WriteStartElement("soap", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
                writer.WriteStartElement("soap", "Fault", "http://schemas.xmlsoap.org/soap/envelope/");
                
                writer.WriteElementString("faultcode", "soap:Server");
                writer.WriteElementString("faultstring", fault.Message);
                
                writer.WriteEndElement(); // Fault
                writer.WriteEndElement(); // Body
                writer.WriteEndElement(); // Envelope
                writer.WriteEndDocument();
                writer.Flush();
            }
        }
    }
    
    // Extension method to register the middleware
    public static class SoapExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseSoapExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SoapExceptionMiddleware>();
        }
    }
}
