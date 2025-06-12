using SoapCore;
using SOAPWebServicesCore.Services;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container - replaces Global.asax.vb Application_Start functionality
builder.Services.AddSingleton<IDataService, DataService>();
builder.Services.AddSingleton<DataService>();

// Add SoapCore services for SOAP functionality
builder.Services.AddSoapCore();

// Configure logging (replaces legacy logging patterns from Global.asax.vb)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline - replaces Global.asax.vb Application_BeginRequest/EndRequest patterns
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Add error handling middleware (replaces Global.asax.vb Application_Error)
app.UseExceptionHandler("/error");

// Configure SOAP endpoint to maintain compatibility with legacy ASMX endpoint
// This replaces the traditional ASMX web service registration from Global.asax.vb
app.UseSoapEndpoint<IDataService>(
    "/DataService.asmx", 
    new SoapEncoderOptions(), 
    SoapSerializer.XmlSerializer);

// Add a root endpoint that returns information about the service
// This provides service discovery functionality that was typically handled in legacy applications
app.MapGet("/", () => "SOAP Web Service is running. Access the service at /DataService.asmx");

// Add health check endpoint for monitoring (modern replacement for legacy application monitoring)
app.MapGet("/health", () => "Healthy");

// Add error handling endpoint
app.MapGet("/error", () => "An error occurred while processing your request.");

// Start the application - replaces Global.asax.vb application lifecycle management
app.Run();