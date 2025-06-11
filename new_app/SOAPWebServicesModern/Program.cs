using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using SOAPWebServicesModern.Contracts;
using SOAPWebServicesModern.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the SOAP service
builder.Services.AddSingleton<GetDataService>();

// Add CoreWCF services
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

// Configure CORS if needed
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Enable WSDL
var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
serviceMetadataBehavior.HttpGetEnabled = true;
serviceMetadataBehavior.HttpGetUrl = new Uri("http://localhost:5000/GetDataService");

// Configure service endpoints
app.UseServiceModel(builder =>
{
    builder.AddService<GetDataService>((serviceOptions) =>
    {
        serviceOptions.DebugBehavior.IncludeExceptionDetailInFaults = true;
    });
    
    // Add a BasicHttpBinding endpoint
    builder.AddServiceEndpoint<GetDataService, IGetDataService>(
        new BasicHttpBinding(),
        "/GetDataService");
});

app.Run();