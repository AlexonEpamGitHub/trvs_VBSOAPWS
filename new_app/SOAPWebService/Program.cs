using SoapCore;
using SOAPWebService.Services;
using System.ServiceModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add logging
builder.Services.AddLogging();

// Add Session configuration with settings from legacy Web.config
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Matching legacy timeout from Web.config
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Authentication configuration for Windows authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = NegotiateDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = NegotiateDefaults.AuthenticationScheme;
})
.AddNegotiate(options =>
{
    // Configure authentication schemes from appsettings.json if needed
    var authConfig = builder.Configuration.GetSection("Authentication");
    if (authConfig.Exists())
    {
        options.EnableLdapClaimResolution = authConfig.GetValue<bool>("EnableLdapClaimResolution", false);
    }
});

// Add Authorization configuration with default policy allowing anonymous access
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAssertion(_ => true) // Allow anonymous access by default
        .Build();
});

// Add CORS configuration using policies from appsettings.json
builder.Services.AddCors(options =>
{
    var corsConfig = builder.Configuration.GetSection("Cors");
    if (corsConfig.Exists())
    {
        var policyName = corsConfig.GetValue<string>("PolicyName", "DefaultPolicy");
        var allowedOrigins = corsConfig.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "*" };
        var allowedMethods = corsConfig.GetSection("AllowedMethods").Get<string[]>() ?? new[] { "GET", "POST" };
        var allowedHeaders = corsConfig.GetSection("AllowedHeaders").Get<string[]>() ?? new[] { "*" };
        
        options.AddPolicy(policyName, policy =>
        {
            if (allowedOrigins.Contains("*"))
            {
                policy.AllowAnyOrigin();
            }
            else
            {
                policy.WithOrigins(allowedOrigins);
            }
            
            if (allowedMethods.Contains("*"))
            {
                policy.AllowAnyMethod();
            }
            else
            {
                policy.WithMethods(allowedMethods);
            }
            
            if (allowedHeaders.Contains("*"))
            {
                policy.AllowAnyHeader();
            }
            else
            {
                policy.WithHeaders(allowedHeaders);
            }
        });
    }
    else
    {
        // Default CORS policy if not configured in appsettings.json
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    }
});

// Add SOAP Service
builder.Services.AddScoped<IGetDataService, GetDataService>();

// Add SoapCore with proper configuration for version 1.1.0.45
builder.Services.AddSoapCore();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// Ensure proper middleware pipeline ordering
app.UseRouting();

// Add CORS middleware
var corsConfig = builder.Configuration.GetSection("Cors");
if (corsConfig.Exists())
{
    var policyName = corsConfig.GetValue<string>("PolicyName", "DefaultPolicy");
    app.UseCors(policyName);
}
else
{
    app.UseCors();
}

// Add Authentication middleware
app.UseAuthentication();

// Add Authorization middleware
app.UseAuthorization();

// Add Session middleware
app.UseSession();

// Configure SOAP endpoint with proper SoapCore 1.1.0.45 syntax for .NET 8 with async support
app.UseEndpoints(endpoints =>
{
    endpoints.UseSoapEndpoint<IGetDataService>("/GetDataService.asmx", new BasicHttpBinding(), SoapSerializer.XmlSerializer);
});

// Add a simple endpoint to show service is running
app.MapGet("/", () => "SOAP Web Service is running. Access the service at /GetDataService.asmx");

app.MapControllers();

app.Run();

// Error handling middleware for proper SOAP fault responses
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request");
            
            if (context.Request.Path.StartsWithSegments("/GetDataService.asmx"))
            {
                // Handle SOAP faults properly
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/xml; charset=utf-8";
                
                var soapFault = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <soap:Fault>
            <faultcode>Server</faultcode>
            <faultstring>Internal Server Error</faultstring>
        </soap:Fault>
    </soap:Body>
</soap:Envelope>";
                
                await context.Response.WriteAsync(soapFault);
            }
            else
            {
                throw;
            }
        }
    }
}