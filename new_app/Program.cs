using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using SOAPWebServices.Core.Contracts;
using SOAPWebServices.Core.Services;
using SOAPWebServices.Core.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Create and initialize the ApplicationEvents instance
ApplicationEvents applicationEvents = new ApplicationEvents();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS if needed
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configure CoreWCF Services
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

// Configure Kestrel to listen on port 8080 (same as the legacy app)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

// Make the ApplicationEvents instance available through DI
builder.Services.AddSingleton(applicationEvents);

var app = builder.Build();

// Call OnApplicationStarting after the app is built but before it starts
applicationEvents.OnApplicationStarting();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthorization();

// Register application lifecycle events middleware
app.Use(async (context, next) => {
    try {
        await next(context);
    }
    catch (Exception ex) {
        applicationEvents.OnError(ex);
        throw;
    }
});

app.MapControllers();

// Configure ServiceModel
app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<GetDataService>();
    serviceBuilder.AddServiceEndpoint<GetDataService, IGetDataService>(
        new BasicHttpBinding(),
        "/GetDataService.svc");

    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
    serviceMetadataBehavior.HttpGetUrl = new Uri("http://localhost:8080/GetDataService");
});

// Register application lifecycle events with the host lifetime
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() => {
    applicationEvents.OnApplicationStarted();
});

lifetime.ApplicationStopping.Register(() => {
    applicationEvents.OnApplicationStopping();
});

lifetime.ApplicationStopped.Register(() => {
    applicationEvents.OnApplicationStopped();
});

// Register application shutdown event for non-graceful shutdowns
AppDomain.CurrentDomain.ProcessExit += (sender, e) => {
    applicationEvents.OnApplicationStopping();
};

app.Run();