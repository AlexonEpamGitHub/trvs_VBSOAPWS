using SoapCore;
using SOAPWebServicesCore.Services;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<IDataService, DataService>();
builder.Services.AddSingleton<DataService>();

// Add SoapCore services
builder.Services.AddSoapCore();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Configure SOAP endpoint to maintain compatibility with legacy ASMX endpoint
app.UseSoapEndpoint<IDataService>(
    "/DataService.asmx", 
    new SoapEncoderOptions(), 
    SoapSerializer.XmlSerializer);

// Add a root endpoint that returns information about the service
app.MapGet("/", () => "SOAP Web Service is running. Access the service at /DataService.asmx");

// Add health check endpoint for monitoring
app.MapGet("/health", () => "Healthy");

app.Run();