using SoapCore;
using SOAPWebServicesCore.Services;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Configure logging (replaces legacy Global.asax application event handling)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services to the container
builder.Services.AddSoapCore();
builder.Services.AddScoped<IDataService, DataService>();

// Configure session state (migrated from Web.config: sessionState mode="InProc" timeout="20")
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure authentication (migrated from Web.config: authentication mode="Windows")
builder.Services.AddAuthentication("Windows")
    .AddNegotiate();

// Configure authorization (migrated from Web.config: allow users="*")
builder.Services.AddAuthorization();

// Add memory cache for session state
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // Migrated from Web.config: customErrors mode="RemoteOnly"
    app.UseExceptionHandler("/Error");
}

// Configure middleware pipeline
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Configure routing
app.UseRouting();

// Configure SOAP endpoint at '/DataService.asmx' with SoapCore middleware
app.UseEndpoints(endpoints =>
{
    endpoints.UseSoapEndpoint<IDataService>(
        path: "/DataService.asmx",
        encoder: new SoapEncoderOptions(),
        serializer: SoapCore.SoapSerializer.XmlSerializer,
        caseInsensitivePath: true
    );
});

// Root path redirect to SOAP service endpoint for convenience
app.MapGet("/", () => Results.Redirect("/DataService.asmx"));

// Health check endpoint for monitoring
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "SOAP Web Services Core" }));

app.Run();
