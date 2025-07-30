using SOAPWebServicesSimple.Services;
using SOAPWebServicesSimple.Middleware;
using SoapCore;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the SOAP service
builder.Services.AddSingleton<IDataService, DataService>();
builder.Services.AddSoapCore();

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

// Add session support
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// Add our custom middleware for application events
app.UseApplicationEvents();

// Add session middleware
app.UseSession();

// Configure SOAP endpoint to match the original .asmx path
app.UseSoapEndpoint<IDataService>("/GetDataService.asmx", new SoapEncoderOptions(), 
    SoapSerializer.DataContractSerializer);

app.UseAuthorization();
app.MapControllers();

app.Run();