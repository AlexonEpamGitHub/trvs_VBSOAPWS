using SoapCore;
using SOAPWebService.Services;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add logging
builder.Services.AddLogging();

// Add SOAP Service
builder.Services.AddScoped<IGetDataService, GetDataService>();

// Add SoapCore with proper configuration for version 1.1.0.45
builder.Services.AddSoapCore();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// Ensure proper middleware ordering - UseRouting must come before UseSoapEndpoint
app.UseRouting();

// Configure SOAP endpoint with proper SoapCore 1.1.0.45 syntax for .NET 8 with async support
app.UseEndpoints(endpoints =>
{
    endpoints.UseSoapEndpoint<IGetDataService>("/GetDataService.asmx", new BasicHttpBinding(), SoapSerializer.XmlSerializer);
});

// Add a simple endpoint to show service is running
app.MapGet("/", () => "SOAP Web Service is running. Access the service at /GetDataService.asmx");

app.MapControllers();

app.Run();

// Error handling middleware for proper SOAP fault responses
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An error occurred while processing the request");
            
            if (context.Request.Path.StartsWithSegments("/GetDataService.asmx"))
            {
                // Handle SOAP faults properly
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/xml; charset=utf-8";
                
                var soapFault = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <soap:Fault>
            <faultcode>Server</faultcode>
            <faultstring>Internal Server Error</faultstring>
        </soap:Fault>
    </soap:Body>
</soap:Envelope>";
                
                await context.Response.WriteAsync(soapFault);
            }
            else
            {
                throw;
            }
        }
    }
}