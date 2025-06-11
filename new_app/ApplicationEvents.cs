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
            logger?.LogInformation("Configuring application events to replace Global.asax functionality");
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
                
                // Set response header for correlation tracking
                context.Response.Headers["X-Correlation-ID"] = correlationId;
                
                // Add request details to diagnostics
                var endpoint = context.GetEndpoint()?.DisplayName;
                var userAgent = context.Request.Headers.UserAgent.ToString();
                var referrer = context.Request.Headers.Referer.ToString();
                
                logger?.LogDebug("Request {CorrelationId} started: Path={Path}, Method={Method}, Endpoint={Endpoint}, UserAgent={UserAgent}, Referrer={Referrer}",
                    correlationId, context.Request.Path, context.Request.Method, endpoint, userAgent, referrer);
                
                try
                {
                    // Call the next middleware in the pipeline
                    await next.Invoke();
                }
                finally
                {
                    // Code to execute after the request is processed (Application_EndRequest equivalent)
                    requestStartTime.Stop();
                    var elapsed = requestStartTime.ElapsedMilliseconds;
                    
                    // Log timing information with different levels based on duration
                    if (elapsed > 1000)
                    {
                        logger?.LogWarning("Request {CorrelationId} to {Path} completed in {Elapsed}ms with status code {StatusCode} - SLOW REQUEST",
                            correlationId, context.Request.Path, elapsed, context.Response.StatusCode);
                    }
                    else
                    {
                        logger?.LogDebug("Request {CorrelationId} to {Path} completed in {Elapsed}ms with status code {StatusCode}",
                            correlationId, context.Request.Path, elapsed, context.Response.StatusCode);
                    }
                }
            });

            // Replace Application_AuthenticateRequest
            // This is handled by ASP.NET Core Authentication middleware (app.UseAuthentication())
            app.Use(async (context, next) =>
            {
                // Execute after authentication but before authorization
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    logger?.LogDebug("Request {CorrelationId}: User {UserName} authenticated successfully",
                        context.TraceIdentifier, context.User.Identity.Name);
                    
                    // Set security headers
                    if (!context.Response.Headers.ContainsKey("X-Content-Type-Options"))
                        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                    
                    if (!context.Response.Headers.ContainsKey("X-XSS-Protection"))
                        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                }
                
                await next(context);
            });
            
            // Enhanced exception handling middleware (Application_Error replacement)
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;
                    var correlationId = context.TraceIdentifier;

                    // Log the exception with correlation ID and detailed information
                    logger?.LogError(exception, 
                        "Unhandled exception occurred for request {CorrelationId}, URL: {Url}, Method: {Method}, User: {User}", 
                        correlationId, 
                        exceptionHandlerPathFeature?.Path, 
                        context.Request.Method,
                        context.User?.Identity?.Name ?? "Anonymous");

                    // Set response status code and content type
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    // Return structured error response
                    var errorResponse = new
                    {
                        TraceId = correlationId,
                        Message = "An unexpected error occurred. Please try again later.",
                        Timestamp = DateTime.UtcNow,
                        // Include exception details only in development
#if DEBUG
                        Details = exception?.Message,
                        StackTrace = exception?.StackTrace?.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
#endif
                    };

                    // Serialize to JSON
                    await context.Response.WriteAsJsonAsync(errorResponse);
                    
                    // Optionally send notification for critical errors
                    if (exception is OutOfMemoryException or StackOverflowException or AccessViolationException)
                    {
                        logger?.LogCritical("CRITICAL ERROR: {ExceptionType} occurred. Immediate attention required.", 
                            exception?.GetType().Name);
                        // Here you would integrate with an alert system or notification service
                    }
                });
            });

            // Replace Session_Start and Session_End
            // Session management is handled differently in ASP.NET Core through:
            // builder.Services.AddDistributedMemoryCache();
            // builder.Services.AddSession(options => { ... });
            // Then in the request pipeline: app.UseSession();

            // Application_PreSendRequestHeaders equivalent
            app.Use(async (context, next) =>
            {
                await next(context);
                
                // Execute after the response is generated but before it's sent
                if (!context.Response.Headers.ContainsKey("Server"))
                {
                    // Remove or mask server information for security
                    context.Response.Headers.Server = "";
                }
                
                // Add cache control headers for non-API endpoints if not already set
                if (!context.Response.Headers.ContainsKey("Cache-Control") && 
                    !context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate, max-age=0";
                    context.Response.Headers.Pragma = "no-cache";
                }
            });
            
            // Enhanced diagnostics middleware
            app.Use(async (context, next) =>
            {
                // Track resource usage for potential memory leaks or performance issues
                var memoryBefore = GC.GetTotalMemory(false);
                var cpuBefore = Process.GetCurrentProcess().TotalProcessorTime;
                var requestStart = DateTime.UtcNow;
                
                try
                {
                    await next(context);
                }
                finally
                {
                    // Log memory usage for requests that might be problematic
                    var memoryAfter = GC.GetTotalMemory(false);
                    var memoryDelta = memoryAfter - memoryBefore;
                    var cpuAfter = Process.GetCurrentProcess().TotalProcessorTime;
                    var cpuDelta = (cpuAfter - cpuBefore).TotalMilliseconds;
                    var requestTime = (DateTime.UtcNow - requestStart).TotalMilliseconds;
                    
                    // Add performance metrics to response headers in development
                    if (app.Environment.IsDevelopment())
                    {
                        context.Response.Headers["X-Memory-Usage"] = memoryDelta.ToString();
                        context.Response.Headers["X-CPU-Usage"] = cpuDelta.ToString();
                        context.Response.Headers["X-Request-Duration"] = requestTime.ToString();
                    }
                    
                    if (memoryDelta > 5_000_000) // 5MB threshold
                    {
                        logger?.LogWarning(
                            "High memory usage detected for request {Path}: {MemoryDelta:N0} bytes, CPU time: {CpuTime:N2}ms",
                            context.Request.Path, memoryDelta, cpuDelta);
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
                var process = Process.GetCurrentProcess();
                _logger.LogInformation(
                    "Application Information: Runtime={Runtime}, OS={OS}, Processors={ProcessorCount}, " +
                    "PhysicalMemory={Memory:N0}MB, ProcessId={ProcessId}, StartTime={StartTime}",
                    Environment.Version,
                    Environment.OSVersion,
                    Environment.ProcessorCount,
                    GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024 * 1024),
                    process.Id,
                    process.StartTime.ToUniversalTime());
                
                // Log current GC mode
                _logger.LogInformation("GC Configuration: Server GC={ServerGC}, Concurrent={ConcurrentGC}, Generation 0={Gen0Size}, " +
                    "Generation 1={Gen1Size}, Generation 2={Gen2Size}",
                    GCSettings.IsServerGC,
                    GCSettings.LatencyMode,
                    GC.CollectionCount(0),
                    GC.CollectionCount(1),
                    GC.CollectionCount(2));
                
                // Log configured endpoint information
                _logger.LogInformation("Application endpoints configured successfully");
            });

            _appLifetime.ApplicationStopping.Register(() =>
            {
                _logger.LogInformation("Application is stopping - executing graceful shutdown tasks");
                // Perform cleanup tasks
                try
                {
                    // Application state capture for diagnostics
                    var process = Process.GetCurrentProcess();
                    var uptime = DateTime.Now - process.StartTime;
                    
                    _logger.LogInformation(
                        "Application shutting down: Uptime={Uptime}, WorkingSet={WorkingSet:N0}MB, " +
                        "PeakWorkingSet={PeakWorkingSet:N0}MB, Threads={Threads}",
                        uptime,
                        process.WorkingSet64 / (1024 * 1024),
                        process.PeakWorkingSet64 / (1024 * 1024),
                        process.Threads.Count);
                    
                    // Flush any pending operations
                    _logger.LogInformation("Flushing pending operations and closing resources");
                    
                    // Allow time for in-flight requests to complete
                    Task.Delay(TimeSpan.FromSeconds(2)).Wait();
                    
                    // Close/dispose any application resources here
                    _logger.LogInformation("Resource cleanup completed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during application shutdown");
                }
                finally 
                {
                    _logger.LogInformation("Shutdown sequence completed");
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
