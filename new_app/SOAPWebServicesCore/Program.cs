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

// Configure SOAP endpoint
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    // Add SOAP service endpoint
    endpoints.UseSoapEndpoint<GetDataService>("/GetDataService.asmx", new SoapEncoderOptions(), 
        SoapSerializer.XmlSerializer);
});

// Map global exception handling (similar to Application_Error in Global.asax)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("An internal server error has occurred.");
    });
});

app.Run();