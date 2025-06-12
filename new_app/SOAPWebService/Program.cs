using SoapCore;
using SOAPWebService.Services;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SOAP Service
builder.Services.AddSingleton<IGetDataService, GetDataService>();

// Add SoapCore
builder.Services.AddSoapCore();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure proper middleware ordering - UseRouting must come before UseSoapEndpoint
app.UseRouting();

// Configure SOAP endpoint with proper SoapCore 1.1.0.51 syntax for .NET 8
app.UseSoapEndpoint<IGetDataService>("/GetDataService.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);

// Add a simple endpoint to show service is running
app.MapGet("/", () => "SOAP Web Service is running. Access the service at /GetDataService.asmx");

app.MapControllers();

app.Run();