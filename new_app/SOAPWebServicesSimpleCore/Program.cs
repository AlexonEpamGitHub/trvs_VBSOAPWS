using Microsoft.AspNetCore.Server.IISIntegration;
using SOAPWebServicesSimpleCore.Interfaces;
using SOAPWebServicesSimpleCore.Middleware;
using SOAPWebServicesSimpleCore.Services;
using SoapCore;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<IDataService, DataService>();
builder.Services.AddSoapCore();

// Add session similar to the legacy app (matches 20-minute timeout from Web.config)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add authentication similar to the legacy Windows auth
builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline

// Custom exception handling for SOAP responses
app.UseExceptionHandlingMiddleware();

// Replace legacy pipeline with modern middleware
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Configure request encoding (replacing globalization settings from Web.config)
app.UseRequestLocalization();

// Map SOAP endpoint to match the legacy .asmx path
app.UseSoapEndpoint<IDataService>("/GetDataService.asmx", new SoapEncoderOptions(), 
    SoapSerializer.DataContractSerializer);

// Optional: Add XML documentation endpoint to match .vsdisco behavior
app.MapGet("/GetDataService.asmx", async (HttpContext context) =>
{
    context.Response.ContentType = "text/xml";
    await context.Response.WriteAsync(@"<?xml version=""1.0"" encoding=""utf-8""?>
<wsdl:definitions xmlns:soap=""http://schemas.xmlsoap.org/wsdl/soap/"" 
                 xmlns:tns=""http://tempuri.org/"" 
                 targetNamespace=""http://tempuri.org/"">
  <!-- Service documentation would go here -->
</wsdl:definitions>");
});

app.Run();