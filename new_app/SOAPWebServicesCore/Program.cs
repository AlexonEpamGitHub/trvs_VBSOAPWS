using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using SOAPWebServicesCore.Middleware;
using SOAPWebServicesCore.Services;
using SoapCore;
using System.Globalization;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SOAP service
builder.Services.AddSingleton<DataService>();
builder.Services.AddSoapCore();

// Configure Windows Authentication (similar to Web.config setting)
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

// Configure session state (similar to the original Web.config sessionState)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure globalization (similar to Web.config globalization settings)
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US") };
    options.SupportedUICultures = new List<CultureInfo> { new CultureInfo("en-US") };
});

// Configure Kestrel to handle synchronous IO (needed for some SOAP operations)
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Configure custom errors (similar to customErrors in Web.config)
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRequestLocalization();
app.UseRouting();

// Add the application lifecycle middleware (Global.asax replacement)
app.UseApplicationLifecycle();

app.UseAuthentication();
app.UseAuthorization();

// Use session (similar to the session configuration in Web.config)
app.UseSession();

// Configure SOAP endpoint - same path as the original .asmx file
app.UseSoapEndpoint<DataService>("/GetDataService.asmx", new SoapEncoderOptions(), 
    SoapSerializer.DataContractSerializer);

// Map controllers for REST API (if needed)
app.MapControllers();

app.Run();