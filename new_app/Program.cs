using SoapCore;
using SOAPWebServicesCore.Services;
using System.ServiceModel;

// Create .NET 8 minimal hosting model application builder
// This replaces the traditional Global.asax.vb Application_Start event handling
var builder = WebApplication.CreateBuilder(args);

// Register dependency injection services - replaces Global.asax.vb service initialization
// Configure IDataService and DataService as singletons for SOAP operations
// The IDataService interface uses System.ServiceModel.ServiceContract attributes
builder.Services.AddSingleton<IDataService, DataService>();
builder.Services.AddSingleton<DataService>();

// Add SoapCore services to enable SOAP functionality in .NET 8
// This provides the infrastructure needed to replace legacy ASMX web services
// SoapCore works with System.ServiceModel.Primitives package for ServiceContract support
builder.Services.AddSoapCore();

// Configure logging system (replaces legacy logging patterns from Global.asax.vb)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Build the web application instance
var app = builder.Build();

// Configure HTTP request pipeline - replaces Global.asax.vb Application_BeginRequest/EndRequest patterns
// Enable developer exception page for development environment debugging
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Configure SOAP endpoint to maintain compatibility with legacy ASMX web service
// This endpoint replaces the traditional .asmx file and Global.asax.vb service registration
// Uses XmlSerializer for backward compatibility with existing SOAP clients
// The IDataService interface is properly recognized through System.ServiceModel attributes
app.UseSoapEndpoint<IDataService>(
    "/DataService.asmx", 
    new SoapEncoderOptions(), 
    SoapSerializer.XmlSerializer);

// Add root endpoint that provides service discovery information
// This replaces functionality that was typically handled in legacy Global.asax.vb applications
app.MapGet("/", () => "SOAP Web Service is running. Access the service at /DataService.asmx");

// Start the application - replaces Global.asax.vb application lifecycle management
// This is the main entry point that keeps the service running
app.Run();