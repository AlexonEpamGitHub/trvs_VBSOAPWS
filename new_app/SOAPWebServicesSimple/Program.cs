using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
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

app.Run();