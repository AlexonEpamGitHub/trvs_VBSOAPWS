using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using SOAPCoreServices.Interfaces;
using SOAPCoreServices.Services;
using SoapCore;
using System;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register SOAP service
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddSoapCore();
builder.Services.AddSoapServiceOperationTuner(new SoapCoreOperationTuner());

// Configure session state
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Windows authentication
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.IISDefaults.AuthenticationScheme)
    .AddNegotiate(); // Add Windows authentication package
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Configure SOAP endpoints
app.UseSoapEndpoint<IDataService>("/GetDataService.asmx", new SoapEncoderOptions 
{ 
    MessageVersion = System.ServiceModel.Channels.MessageVersion.Soap12WSAddressingAugust2004,
    WriteEncoding = System.Text.Encoding.UTF8,
    ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas
    {
        MaxStringContentLength = 1024 * 1024
    }
}, SoapSerializer.DataContractSerializer);

// Add WSDL service description endpoint
app.UseSoapEndpoint<IDataService>("/GetDataService.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer, true);

// Configure middleware
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllers();

// Add error handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("An error occurred. Please try again later.");
    });
});

// SoapCore operation tuner class for customizing SOAP operations
public class SoapCoreOperationTuner : ISoapCoreOperationTuner
{
    public void Tune(HttpContext httpContext, object serviceInstance, SoapCore.ServiceModel.OperationDescription operation)
    {
        // Customize SOAP operation behavior here if needed
    }
}

app.Run();