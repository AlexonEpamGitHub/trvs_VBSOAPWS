using SOAPWebServicesSimpleCore.Interfaces;
using SOAPWebServicesSimpleCore.Services;
using SOAPWebServicesSimpleCore.Middleware;
using SoapCore;
using System.ServiceModel;
using Microsoft.AspNetCore.Server.IISIntegration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SOAP service
builder.Services.AddSingleton<IDataService, DataService>();
builder.Services.AddSoapCore();

// Add authentication
builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

// Add session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandlingMiddleware();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Configure globalization
app.UseRequestLocalization();

// Map SOAP endpoints
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