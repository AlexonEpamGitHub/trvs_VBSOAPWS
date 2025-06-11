using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SOAPWebServices.Core
{
    /// <summary>
    /// Handles application events that were previously managed in Global.asax
    /// in the legacy VB.NET application. In ASP.NET Core, these events are
    /// now managed through middleware components.
    /// </summary>
    public static class ApplicationEvents
    {
        /// <summary>
        /// Configures application lifecycle events in ASP.NET Core
        /// that replace Global.asax events from .NET Framework
        /// </summary>
        /// <param name="app">The WebApplication instance to configure</param>
        /// <param name="logger">Optional logger instance for logging events</param>
        public static void ConfigureApplicationEvents(WebApplication app, ILogger? logger = null)
        {
            // Replace Application_Start
            // Already handled in Program.cs through builder.Build() and configuration methods

            // Replace Application_BeginRequest
            app.Use(async (context, next) =>
            {
                // Code to execute at the beginning of each request
                var requestStartTime = Stopwatch.StartNew();
                
                // Call the next middleware in the pipeline
                await next.Invoke();
                
                // Code to execute after the request is processed
                requestStartTime.Stop();
                logger?.LogDebug($"Request to {context.Request.Path} completed in {requestStartTime.ElapsedMilliseconds}ms");
            });

            // Replace Application_AuthenticateRequest
            // This is handled by ASP.NET Core Authentication middleware (app.UseAuthentication())
            
            // Replace Application_Error
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;

                    // Log the exception
                    logger?.LogError(exception, "Unhandled exception occurred");

                    // You can customize the error response here
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
                });
            });

            // Replace Session_Start and Session_End
            // Session management is handled differently in ASP.NET Core through:
            // builder.Services.AddDistributedMemoryCache();
            // builder.Services.AddSession(options => { ... });
            // Then in the request pipeline: app.UseSession();

            // Replace Application_End
            // This is handled by host lifetime events:
            // builder.Services.AddHostedService<ApplicationLifetimeService>();
        }
    }

    /// <summary>
    /// Optional service to handle application lifetime events
    /// </summary>
    public class ApplicationLifetimeService : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger<ApplicationLifetimeService> _logger;

        public ApplicationLifetimeService(
            IHostApplicationLifetime appLifetime,
            ILogger<ApplicationLifetimeService> logger)
        {
            _appLifetime = appLifetime;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(() =>
            {
                _logger.LogInformation("Application started");
                // Application_Start equivalent
            });

            _appLifetime.ApplicationStopping.Register(() =>
            {
                _logger.LogInformation("Application is stopping");
                // Code to run before shutdown
            });

            _appLifetime.ApplicationStopped.Register(() =>
            {
                _logger.LogInformation("Application stopped");
                // Application_End equivalent
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}