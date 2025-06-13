using SOAPWebServicesSimple.Services;
using SoapCore;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSingleton<IDataService, DataService>();

// Add logging
builder.Services.AddLogging();

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Enable routing middleware
app.UseRouting();
app.UseAuthorization();

// Configure SOAP endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.UseSoapEndpoint<IDataService>("/GetDataService.asmx", new SoapEncoderOptions(), 
        SoapSerializer.DataContractSerializer);
    
    // Map controllers
    endpoints.MapControllers();
});

app.Run();