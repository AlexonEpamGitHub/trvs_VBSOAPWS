using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SOAPCoreServices.Interfaces;
using SOAPCoreServices.Services;
using SoapCore;
using System;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register SOAP service
builder.Services.AddSingleton<IDataService, DataService>();
builder.Services.AddSoapCore();

// Configure session state (matching legacy configuration)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Authentication (Windows authentication from legacy app)
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.IISDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Configure SOAP endpoints - this replaces the .asmx file
app.UseSoapEndpoint<IDataService>("/GetDataService.asmx", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);

// Configure middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllers();

// Add error handling similar to the legacy Global.asax Application_Error
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("An error occurred. Please try again later.");
    });
});

app.Run();