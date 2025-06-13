using SOAPWebServicesSimple.Services;
using SoapCore;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSingleton<IDataService, DataService>();

// Configure session state based on Web.config settings
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Matches the 20-minute timeout from Web.config
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // This matches the RemoteOnly custom errors mode from Web.config
    app.UseExceptionHandler("/Error");
}

app.UseRouting();
app.UseAuthorization();
app.UseSession();

// Configure SOAP endpoints
app.UseEndpoints(endpoints =>
{
    // Register the SOAP service at the same path as the original .asmx file
    endpoints.UseSoapEndpoint<IDataService>("/GetDataService.asmx", new SoapEncoderOptions(),
        SoapSerializer.DataContractSerializer);
    
    endpoints.MapControllers();
});

app.Run();