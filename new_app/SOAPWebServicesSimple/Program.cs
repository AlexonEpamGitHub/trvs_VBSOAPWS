using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.AspNetCore.Server.IISIntegration;
using SOAPWebServicesSimple;
using SOAPWebServicesSimple.Middleware;
using SOAPWebServicesSimple.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CoreWCF services
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

// Configure Windows Authentication
builder.Services.Configure<IISOptions>(options => 
{
    options.AutomaticAuthentication = true;
});
builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);

// Execute Application_Start equivalent
GlobalApplication.OnApplicationStart();

// Register session services to replace Session_Start/End handlers
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // From Web.config sessionState timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Register the Global.asax event handlers middleware
app.UseGlobalEvents();

// Enable session to handle Session_Start and Session_End events
app.UseSession();

app.UseAuthentication(); // Add this line before UseAuthorization
app.UseAuthorization();
app.MapControllers();

// Configure CoreWCF services
app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<DataService>();
    serviceBuilder.AddServiceEndpoint<DataService, IDataService>(new BasicHttpBinding(), "/GetDataService.svc");
    
    ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior
    {
        HttpGetEnabled = true,
        HttpsGetEnabled = true
    };
    
    serviceBuilder.ConfigureServiceHostBase<DataService>(host => host.Description.Behaviors.Add(metadataBehavior));
});

// Register application lifetime events
app.Lifetime.ApplicationStopping.Register(() => 
{
    GlobalApplication.OnApplicationEnd();
});

app.Run();