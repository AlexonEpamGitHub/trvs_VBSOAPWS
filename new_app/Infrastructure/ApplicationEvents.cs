using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SOAPWebServices.Core.Infrastructure
{
    /// <summary>
    /// Handles application events that were previously managed in Global.asax
    /// in the legacy VB.NET application. In ASP.NET Core, these events are
    /// now managed through middleware components.
    /// </summary>
    public class ApplicationEvents
    {
        private ILogger<ApplicationEvents>? _logger;
        private DateTime _startTime;

        public ApplicationEvents()
        {
            _startTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Setup logger if available
        /// </summary>
        /// <param name="logger">The logger instance</param>
        public void SetLogger(ILogger<ApplicationEvents> logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Fires when the application is starting, equivalent to Application_Start in Global.asax
        /// </summary>
        public void OnApplicationStarting()
        {
            _logger?.LogInformation("Application starting at {StartTime}", _startTime);
            _logger?.LogInformation("Runtime: {Runtime}, OS: {OS}, Processors: {ProcessorCount}",
                Environment.Version,
                Environment.OSVersion,
                Environment.ProcessorCount);
            
            // Add any initialization logic here that was previously in Application_Start
        }
        
        /// <summary>
        /// Fires when an error occurs, equivalent to Application_Error in Global.asax
        /// </summary>
        /// <param name="exception">The exception that occurred</param>
        public void OnError(Exception exception)
        {
            _logger?.LogError(exception, "Unhandled exception occurred in application");
            
            // Add any error handling logic here that was previously in Application_Error
        }
        
        /// <summary>
        /// Fires when the application is stopping, equivalent to Application_End in Global.asax
        /// </summary>
        public void OnApplicationStopping()
        {
            var uptime = DateTime.UtcNow - _startTime;
            _logger?.LogInformation("Application stopping. Uptime: {Uptime}", uptime);
            
            // Add any cleanup logic here that was previously in Application_End
            var process = Process.GetCurrentProcess();
            _logger?.LogInformation(
                "Application shutting down: WorkingSet={WorkingSet:N0}MB, PeakWorkingSet={PeakWorkingSet:N0}MB, Threads={Threads}",
                process.WorkingSet64 / (1024 * 1024),
                process.PeakWorkingSet64 / (1024 * 1024),
                process.Threads.Count);
        }
        
        /// <summary>
        /// Configure the application request pipeline with middleware that replaces Global.asax events
        /// </summary>
        /// <param name="app">The WebApplication instance</param>
        public void ConfigureApplicationPipeline(WebApplication app)
        {
            // Configure logger if not already set
            if (_logger == null)
            {
                _logger = app.Services.GetService<ILogger<ApplicationEvents>>();
            }
            
            // Replace Application_BeginRequest
            app.Use(async (context, next) =>
            {
                // Code to execute at the beginning of each request
                var requestStartTime = Stopwatch.StartNew();
                
                // Add request correlation ID for tracking
                var correlationId = context.TraceIdentifier;
                context.Items["CorrelationId"] = correlationId;
                
                _logger?.LogDebug("Request {CorrelationId} to {Path} started", 
                    correlationId, context.Request.Path);
                
                try
                {
                    // Call the next middleware in the pipeline
                    await next.Invoke();
                }
                finally
                {
                    // Code to execute after the request is processed
                    requestStartTime.Stop();
                    _logger?.LogDebug("Request {CorrelationId} to {Path} completed in {ElapsedMs}ms with status code {StatusCode}",
                        correlationId, context.Request.Path, requestStartTime.ElapsedMilliseconds, context.Response.StatusCode);
                }
            });
            
            // Replace Application_AuthenticateRequest
            // This is primarily handled by ASP.NET Core Authentication middleware (app.UseAuthentication())
            app.Use(async (context, next) =>
            {
                // Execute after authentication but before authorization
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    _logger?.LogDebug("User {UserName} authenticated for request {Path}",
                        context.User.Identity.Name, context.Request.Path);
                }
                
                await next.Invoke();
            });
            
            // Replace Application_Error
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;
                    var correlationId = context.TraceIdentifier;

                    // Log the exception
                    _logger?.LogError(exception, "Unhandled exception occurred for request {CorrelationId} to {Path}",
                        correlationId, exceptionHandlerPathFeature?.Path);

                    // You can customize the error response here
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    
                    // Return structured error response
                    var errorResponse = new
                    {
                        TraceId = correlationId,
                        Message = "An unexpected error occurred. Please try again later.",
                        Timestamp = DateTime.UtcNow
                    };
                    
                    await context.Response.WriteAsJsonAsync(errorResponse);
                    
                    // Call OnError to handle as in Application_Error
                    OnError(exception ?? new Exception("Unknown error occurred"));
                });
            });
            
            // Add any other middleware that replaces Global.asax functionality
        }
        
        /// <summary>
        /// Fires when a new session starts, equivalent to Session_Start in Global.asax
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        public void OnSessionStarting(string sessionId)
        {
            _logger?.LogDebug("Session {SessionId} started", sessionId);
            
            // Add any session start logic here that was previously in Session_Start
        }
        
        /// <summary>
        /// Fires when a session ends, equivalent to Session_End in Global.asax
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        public void OnSessionEnding(string sessionId)
        {
            _logger?.LogDebug("Session {SessionId} ended", sessionId);
            
            // Add any session end logic here that was previously in Session_End
        }
    }
}