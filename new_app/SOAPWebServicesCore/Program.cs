using Microsoft.AspNetCore.Server.Kestrel.Core;
using SoapCore;
using SOAPWebServicesCore.Services;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Kestrel to support synchronous IO for SOAP
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

// Register the SOAP service
builder.Services.AddSingleton<IDataService, DataService>();

// Add services for SOAP
builder.Services.AddSoapServiceOperationTuner(new SoapServiceOperationTuner());

// Add Authentication services if needed
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add SOAP endpoint
app.UseSoapEndpoint<IDataService>("/GetDataService.asmx", new SoapEncoderOptions(), 
    SoapSerializer.DataContractSerializer);

app.UseAuthentication();
app.UseAuthorization();

app.Run();

// Add this class to customize SOAP operations if needed
public class SoapServiceOperationTuner : IServiceOperationTuner
{
    public void Tune(OperationDescription operation)
    {
        // Can be used to customize SOAP operations if needed
    }
}