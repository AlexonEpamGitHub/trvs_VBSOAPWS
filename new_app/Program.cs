using SoapCore;
using SOAPWebServicesCore.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSoapCore();
builder.Services.AddScoped<IDataService, DataService>();

// Configure logging (migrated from Web.config compilation debug="true")
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configure session state (migrated from Web.config sessionState)
// Original: <sessionState mode="InProc" timeout="20"/>
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("Session:Timeout", 20));
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add support for controllers and API endpoints if needed for diagnostics
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline

// Development exception page (migrated from Web.config customErrors mode="RemoteOnly")
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // In production, use custom error handling instead of RemoteOnly
    app.UseExceptionHandler("/Error");
}

// Enable session middleware (migrated from Web.config sessionState configuration)
app.UseSession();

// Configure routing for SOAP endpoint
app.UseRouting();

// Configure SOAP endpoint at '/DataService.asmx' path
// This replaces the legacy GetDataService.asmx endpoint
app.UseEndpoints(endpoints =>
{
    endpoints.UseSoapEndpoint<IDataService>(
        path: "/DataService.asmx",
        encoder: new SoapEncoderOptions(),
        serializer: SoapCore.SoapSerializer.XmlSerializer,
        caseInsensitivePath: true
    );
    
    // Optional: Add controllers for any additional REST endpoints
    endpoints.MapControllers();
});

// Add a redirect from root to the service endpoint for convenience
// This helps with discovery and testing of the SOAP service
app.MapGet("/", () => Results.Redirect("/DataService.asmx"));

// Add a simple health check endpoint for monitoring
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

app.Run();