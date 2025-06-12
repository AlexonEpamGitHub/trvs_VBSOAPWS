using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using SOAPWebServicesCore.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add CoreWCF services
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

// Add session support (equivalent to web.config sessionState)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// Configure SOAP endpoints
app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<GetDataService>();
    serviceBuilder.AddServiceEndpoint<GetDataService, IGetDataService>(
        new BasicHttpBinding(), 
        "/GetDataService.svc");
        
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
    serviceMetadataBehavior.HttpGetUrl = new Uri("/GetDataService.svc", UriKind.Relative);
});

app.Run();