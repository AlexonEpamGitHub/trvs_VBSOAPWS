using SoapCore;
using SOAPWebService.Services;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container - replaces legacy Global.asax Application_Start functionality
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SOAP Service registration - replaces legacy service instantiation
builder.Services.AddSingleton<IGetDataService, GetDataService>();

// Add SoapCore services for SOAP endpoint support
builder.Services.AddSoapCore();

var app = builder.Build();

// Configure the HTTP request pipeline - replaces legacy Global.asax pipeline configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure routing middleware
app.UseRouting();

// Configure SOAP endpoint at /GetDataService.asmx with XmlSerializer - replaces legacy .asmx handler
app.UseSoapEndpoint<IGetDataService>("/GetDataService.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);

// Add root endpoint with service status - replaces legacy default page functionality
app.MapGet("/", () => "SOAP Web Service is running. Access the service at /GetDataService.asmx");

// Map controllers - enables REST API endpoints alongside SOAP
app.MapControllers();

// Start the application - replaces legacy Global.asax Application lifecycle
app.Run();
