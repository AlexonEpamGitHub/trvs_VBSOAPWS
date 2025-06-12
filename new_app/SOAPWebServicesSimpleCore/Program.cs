using SOAPWebServicesSimpleCore.Interfaces;
using SOAPWebServicesSimpleCore.Services;
using SOAPWebServicesSimpleCore.Middleware;
using SoapCore;
using System.ServiceModel;
using Microsoft.AspNetCore.Server.IISIntegration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<IDataService, DataService>();
builder.Services.AddSoapCore();

// Add session similar to the legacy app
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add authentication similar to the legacy Windows auth
builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

var app = builder.Build();

// Add global exception handling (replacing customErrors mode="RemoteOnly")
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

// Replace legacy pipeline with modern middleware
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Configure request encoding (replacing globalization settings)
app.UseRequestLocalization();

// Map SOAP endpoint to match the legacy .asmx path
app.UseSoapEndpoint<IDataService>("/GetDataService.asmx", new SoapEncoderOptions(), 
    SoapSerializer.DataContractSerializer);

app.Run();