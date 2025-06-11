using SOAPWebServicesCore.Services;
using SoapCore;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the SOAP service
builder.Services.AddSingleton<GetDataService>();
builder.Services.AddSingleton<IGetDataService, GetDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add middleware in the correct order
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Configure SOAP endpoint
app.UseEndpoints(endpoints =>
{
    // Add SOAP service endpoint with the same path as the original (.asmx)
    endpoints.UseSoapEndpoint<IGetDataService>("/GetDataService.asmx", 
        new SoapEncoderOptions(), 
        SoapSerializer.XmlSerializer);
    
    // Map controller endpoints if needed
    endpoints.MapControllers();
});

// Map global exception handling (similar to Application_Error in Global.asax)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("An internal server error has occurred.");
        
        // Additional logging could be added here
    });
});

app.Run();