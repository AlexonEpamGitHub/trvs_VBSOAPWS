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
                
                // Add request correlation ID for tracking
                var correlationId = context.TraceIdentifier;
                context.Items["CorrelationId"] = correlationId;
                logger?.LogDebug($"Request {correlationId} to {context.Request.Path} started");
                
                try
                {
                    // Call the next middleware in the pipeline
                    await next.Invoke();
                }
                finally
                {
                    // Code to execute after the request is processed
                    requestStartTime.Stop();
                    logger?.LogDebug($"Request {correlationId} to {context.Request.Path} completed in {requestStartTime.ElapsedMilliseconds}ms with status code {context.Response.StatusCode}");
                }
            });

            // Replace Application_AuthenticateRequest
            // This is handled by ASP.NET Core Authentication middleware (app.UseAuthentication())
            
            // Enhanced exception handling middleware
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;
                    var correlationId = context.TraceIdentifier;

                    // Log the exception with correlation ID
                    logger?.LogError(exception, "Unhandled exception occurred for request {CorrelationId}", correlationId);

                    // Set response status code and content type
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    // Return structured error response
                    var errorResponse = new
                    {
                        TraceId = correlationId,
                        Message = "An unexpected error occurred. Please try again later.",
                        Timestamp = DateTime.UtcNow
                    };

                    // Serialize to JSON
                    await context.Response.WriteAsJsonAsync(errorResponse);
                });
            });

            // Replace Session_Start and Session_End
            // Session management is handled differently in ASP.NET Core through:
            // builder.Services.AddDistributedMemoryCache();
            // builder.Services.AddSession(options => { ... });
            // Then in the request pipeline: app.UseSession();

            // Enhanced diagnostics middleware
            app.Use(async (context, next) =>
            {
                // Track resource usage for potential memory leaks or performance issues
                var memoryBefore = GC.GetTotalMemory(false);
                
                try
                {
                    await next(context);
                }
                finally
                {
                    // Log memory usage for requests that might be problematic
                    var memoryAfter = GC.GetTotalMemory(false);
                    var memoryDelta = memoryAfter - memoryBefore;
                    
                    if (memoryDelta > 5_000_000) // 5MB threshold
                    {
                        logger?.LogWarning(
                            "High memory usage detected for request {Path}: {MemoryDelta} bytes",
                            context.Request.Path, memoryDelta);
                    }
                }
            });

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
        private readonly IHostEnvironment _environment;

        public ApplicationLifetimeService(
            IHostApplicationLifetime appLifetime,
            ILogger<ApplicationLifetimeService> logger,
            IHostEnvironment environment)
        {
            _appLifetime = appLifetime;
            _logger = logger;
            _environment = environment;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(() =>
            {
                _logger.LogInformation("Application started in {Environment} environment", 
                    _environment.EnvironmentName);
                
                // Log system information for diagnostics
                _logger.LogInformation("Runtime: {Runtime}, OS: {OS}, Processors: {ProcessorCount}",
                    Environment.Version,
                    Environment.OSVersion,
                    Environment.ProcessorCount);
                
                // Application_Start equivalent
            });

            _appLifetime.ApplicationStopping.Register(() =>
            {
                _logger.LogInformation("Application is stopping");
                // Perform cleanup tasks
                try
                {
                    // Flush any pending operations
                    _logger.LogInformation("Flushing pending operations");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during application shutdown");
                }
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
            _logger.LogInformation("Application stopping gracefully...");
            return Task.CompletedTask;
        }
    }
}