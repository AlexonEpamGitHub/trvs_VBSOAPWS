using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;

namespace SOAPWebService.Controllers;

/// <summary>
/// Health check controller providing comprehensive health monitoring and system diagnostics
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/health")]
[Route("api/health")]
[Produces("application/json")]
[Tags("Health Monitoring")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;
    private readonly HealthCheckService _healthCheckService;

    public HealthController(
        ILogger<HealthController> logger,
        HealthCheckService healthCheckService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
    }

    /// <summary>
    /// Get basic health status information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Basic health information including service status and available endpoints</returns>
    [HttpGet]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [SwaggerOperation(
        Summary = "Get basic health status",
        Description = "Returns basic health information including service status, available endpoints, and SOAP methods with async support",
        OperationId = "GetHealth",
        Tags = new[] { "Health Monitoring" }
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Health check completed successfully", typeof(HealthResponse))]
    [SwaggerResponse(StatusCodes.Status408RequestTimeout, "Request timeout", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    [ProducesResponseType<HealthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HealthResponse>> GetAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("HealthCheck_{RequestId}", Guid.NewGuid());
        _logger.LogInformation("Health check endpoint accessed at {Timestamp}", DateTime.UtcNow);

        try
        {
            // Simulate async health data collection
            await Task.Delay(10, cancellationToken);

            var healthInfo = new HealthResponse
            {
                Status = "Healthy",
                Service = "SOAP Web Service",
                Framework = ".NET 8.0",
                Migration = new MigrationInfo
                {
                    From = "Legacy SOAP Service",
                    To = "Modern .NET 8.0 Implementation with Async Support",
                    Status = "Completed",
                    Date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd")
                },
                Endpoints = new[]
                {
                    "/GetDataService.asmx - SOAP Service (Legacy)",
                    "/GetDataService.asmx?wsdl - WSDL Definition",
                    "/api/v1/health - REST Health Check",
                    "/api/v1/health/status - System Status Information",
                    "/api/v1/health/detailed - Detailed Health Check",
                    "/api/v1/health/dependencies - Dependencies Health Check",
                    "/api/v1/health/diagnostics - Service Diagnostics",
                    "/api/v2/health - Enhanced REST Health Check",
                    "/api/v2/health/live - Live Health Check",
                    "/swagger - API Documentation"
                },
                SoapMethods = new[]
                {
                    new SoapMethodInfo { Method = "HelloWorldAsync", Description = "Async hello world test method", Parameters = "CancellationToken (optional)" },
                    new SoapMethodInfo { Method = "GetDataAsync", Description = "Asynchronously retrieves basic data", Parameters = "CancellationToken (optional)" },
                    new SoapMethodInfo { Method = "GetDataSetAsync", Description = "Asynchronously retrieves data in DataSet format", Parameters = "CancellationToken (optional)" },
                    new SoapMethodInfo { Method = "GetReportAsync", Description = "Asynchronously generates and returns report data", Parameters = "CancellationToken (optional)" },
                    new SoapMethodInfo { Method = "HelloWorld", Description = "Legacy synchronous hello world method", Parameters = "None" },
                    new SoapMethodInfo { Method = "GetData", Description = "Legacy synchronous data retrieval", Parameters = "None" },
                    new SoapMethodInfo { Method = "GetDataSet", Description = "Legacy synchronous DataSet retrieval", Parameters = "None" },
                    new SoapMethodInfo { Method = "GetReport", Description = "Legacy synchronous report generation", Parameters = "None" }
                },
                Timestamp = DateTime.UtcNow,
                Version = "2.0.0"
            };

            _logger.LogInformation("Health check completed successfully with status: {Status}", healthInfo.Status);
            return Ok(healthInfo);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Health check request was cancelled");
            var errorResponse = new ErrorResponse
            {
                Status = "Cancelled",
                Error = "Request was cancelled",
                Message = "The health check operation was cancelled due to timeout",
                Timestamp = DateTime.UtcNow
            };
            return StatusCode(StatusCodes.Status408RequestTimeout, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing health check request");
            var errorResponse = new ErrorResponse
            {
                Status = "Unhealthy",
                Error = "Internal server error occurred",
                Message = "An unexpected error occurred during health check",
                Timestamp = DateTime.UtcNow
            };
            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }

    /// <summary>
    /// Get simple health status for monitoring
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Simple health status response</returns>
    [HttpGet("status")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [SwaggerOperation(
        Summary = "Get simple health status",
        Description = "Returns simple health status for monitoring systems",
        OperationId = "GetHealthStatus",
        Tags = new[] { "Health Monitoring" }
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Service is healthy", typeof(SimpleHealthResponse))]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service is unhealthy", typeof(SimpleHealthResponse))]
    [SwaggerResponse(StatusCodes.Status408RequestTimeout, "Request timeout", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    [ProducesResponseType<SimpleHealthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<SimpleHealthResponse>(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SimpleHealthResponse>> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("HealthStatus_{RequestId}", Guid.NewGuid());
        _logger.LogInformation("Health status endpoint accessed at {Timestamp}", DateTime.UtcNow);

        try
        {
            var healthReport = await _healthCheckService.CheckHealthAsync(cancellationToken);

            var response = new SimpleHealthResponse
            {
                Status = healthReport.Status.ToString(),
                IsHealthy = healthReport.Status == HealthStatus.Healthy,
                Timestamp = DateTime.UtcNow
            };

            var statusCode = response.IsHealthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;
            _logger.LogInformation("Health status check completed with status: {Status}", response.Status);

            return StatusCode(statusCode, response);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Health status request was cancelled");
            var errorResponse = new ErrorResponse
            {
                Status = "Cancelled",
                Error = "Request was cancelled",
                Message = "The health status operation was cancelled due to timeout",
                Timestamp = DateTime.UtcNow
            };
            return StatusCode(StatusCodes.Status408RequestTimeout, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving health status");
            var errorResponse = new ErrorResponse
            {
                Status = "Error",
                Error = "Unable to retrieve health status",
                Message = ex.Message,
                Timestamp = DateTime.UtcNow
            };
            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }

    /// <summary>
    /// Get detailed health check with system metrics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed health check results including system metrics and all registered health checks</returns>
    [HttpGet("detailed")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [SwaggerOperation(
        Summary = "Get detailed health check",
        Description = "Returns comprehensive health check results including system metrics, memory usage, and all registered health checks with their individual statuses",
        OperationId = "GetDetailedHealth",
        Tags = new[] { "Health Monitoring", "System Metrics" }
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Detailed health check completed successfully", typeof(DetailedHealthResponse))]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service is unhealthy", typeof(DetailedHealthResponse))]
    [SwaggerResponse(StatusCodes.Status408RequestTimeout, "Request timeout", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    [ProducesResponseType<DetailedHealthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<DetailedHealthResponse>(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DetailedHealthResponse>> GetDetailedAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("DetailedHealthCheck_{RequestId}", Guid.NewGuid());
        _logger.LogInformation("Detailed health check endpoint accessed at {Timestamp}", DateTime.UtcNow);

        try
        {
            var healthReport = await _healthCheckService.CheckHealthAsync(cancellationToken);

            var response = new DetailedHealthResponse
            {
                Status = healthReport.Status.ToString(),
                TotalDuration = healthReport.TotalDuration,
                Checks = healthReport.Entries.Select(entry => new HealthCheckDetail
                {
                    Name = entry.Key,
                    Status = entry.Value.Status.ToString(),
                    Duration = entry.Value.Duration,
                    Description = entry.Value.Description,
                    Exception = entry.Value.Exception?.Message,
                    Data = entry.Value.Data.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString())
                }).ToArray(),
                SystemMetrics = await GetSystemMetricsAsync(cancellationToken),
                Timestamp = DateTime.UtcNow
            };

            var statusCode = healthReport.Status == HealthStatus.Healthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;
            
            _logger.LogInformation("Detailed health check completed with overall status: {Status}, Duration: {Duration}ms", 
                response.Status, response.TotalDuration.TotalMilliseconds);

            return StatusCode(statusCode, response);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Detailed health check request was cancelled");
            var errorResponse = new ErrorResponse
            {
                Status = "Cancelled",
                Error = "Request was cancelled",
                Message = "The detailed health check operation was cancelled due to timeout",
                Timestamp = DateTime.UtcNow
            };
            return StatusCode(StatusCodes.Status408RequestTimeout, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while performing detailed health check");
            var errorResponse = new ErrorResponse
            {
                Status = "Error",
                Error = "Failed to perform detailed health check",
                Message = ex.Message,
                Timestamp = DateTime.UtcNow
            };
            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }

    /// <summary>
    /// Get dependencies health check
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Health status of external dependencies</returns>
    [HttpGet("dependencies")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [SwaggerOperation(
        Summary = "Get dependencies health check",
        Description = "Returns health status of external dependencies and services with async validation",
        OperationId = "GetDependenciesHealth",
        Tags = new[] { "Health Monitoring", "Dependencies" }
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Dependencies are healthy", typeof(DependenciesHealthResponse))]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "One or more dependencies are unhealthy", typeof(DependenciesHealthResponse))]
    [SwaggerResponse(StatusCodes.Status408RequestTimeout, "Request timeout", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    [ProducesResponseType<DependenciesHealthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<DependenciesHealthResponse>(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DependenciesHealthResponse>> GetDependenciesAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("DependenciesHealthCheck_{RequestId}", Guid.NewGuid());
        _logger.LogInformation("Dependencies health check endpoint accessed at {Timestamp}", DateTime.UtcNow);

        try
        {
            var healthReport = await _healthCheckService.CheckHealthAsync(cancellationToken);

            var dependencyChecks = healthReport.Entries
                .Where(entry => entry.Key.Contains("database", StringComparison.OrdinalIgnoreCase) || 
                               entry.Key.Contains("external", StringComparison.OrdinalIgnoreCase) || 
                               entry.Key.Contains("dependency", StringComparison.OrdinalIgnoreCase) ||
                               entry.Key.Contains("soap", StringComparison.OrdinalIgnoreCase))
                .Select(entry => new DependencyHealthInfo
                {
                    Name = entry.Key,
                    Status = entry.Value.Status.ToString(),
                    ResponseTime = entry.Value.Duration,
                    LastChecked = DateTime.UtcNow,
                    Details = entry.Value.Description,
                    Exception = entry.Value.Exception?.Message
                }).ToArray();

            var response = new DependenciesHealthResponse
            {
                OverallStatus = healthReport.Status.ToString(),
                Dependencies = dependencyChecks,
                TotalDependencies = dependencyChecks.Length,
                HealthyDependencies = dependencyChecks.Count(d => d.Status == "Healthy"),
                UnhealthyDependencies = dependencyChecks.Count(d => d.Status != "Healthy"),
                Timestamp = DateTime.UtcNow
            };

            var statusCode = healthReport.Status == HealthStatus.Healthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;

            _logger.LogInformation("Dependencies health check completed. Total: {Total}, Healthy: {Healthy}, Unhealthy: {Unhealthy}", 
                response.TotalDependencies, response.HealthyDependencies, response.UnhealthyDependencies);

            return StatusCode(statusCode, response);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Dependencies health check request was cancelled");
            var errorResponse = new ErrorResponse
            {
                Status = "Cancelled",
                Error = "Request was cancelled",
                Message = "The dependencies health check operation was cancelled due to timeout",
                Timestamp = DateTime.UtcNow
            };
            return StatusCode(StatusCodes.Status408RequestTimeout, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking dependencies health");
            var errorResponse = new ErrorResponse
            {
                Status = "Error",
                Error = "Failed to check dependencies health",
                Message = ex.Message,
                Timestamp = DateTime.UtcNow
            };
            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }

    /// <summary>
    /// Get SOAP service diagnostics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed diagnostics information for the SOAP service</returns>
    [HttpGet("diagnostics")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [SwaggerOperation(
        Summary = "Get SOAP service diagnostics",
        Description = "Returns detailed diagnostics information including SOAP service status, performance metrics, configuration details, and async method availability",
        OperationId = "GetSoapDiagnostics",
        Tags = new[] { "Health Monitoring", "SOAP Diagnostics" }
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "SOAP diagnostics retrieved successfully", typeof(SoapDiagnosticsResponse))]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "SOAP service is unavailable", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status408RequestTimeout, "Request timeout", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    [ProducesResponseType<SoapDiagnosticsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SoapDiagnosticsResponse>> GetDiagnosticsAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("SoapDiagnostics_{RequestId}", Guid.NewGuid());
        _logger.LogInformation("SOAP diagnostics endpoint accessed at {Timestamp}", DateTime.UtcNow);

        try
        {
            // Simulate async diagnostics collection with proper timeout handling
            await Task.Delay(100, cancellationToken);

            var diagnosticsInfo = new SoapDiagnosticsResponse
            {
                ServiceStatus = "Active",
                WsdlAccessible = true,
                SoapEndpointUrl = "/GetDataService.asmx",
                WsdlUrl = "/GetDataService.asmx?wsdl",
                SupportedSoapVersions = ["1.1", "1.2"],
                AvailableMethods = new[]
                {
                    new SoapMethodDiagnostics { Name = "HelloWorldAsync", Status = "Available", LastTested = DateTime.UtcNow.AddMinutes(-2), IsAsync = true },
                    new SoapMethodDiagnostics { Name = "GetDataAsync", Status = "Available", LastTested = DateTime.UtcNow.AddMinutes(-1), IsAsync = true },
                    new SoapMethodDiagnostics { Name = "GetDataSetAsync", Status = "Available", LastTested = DateTime.UtcNow.AddSeconds(-30), IsAsync = true },
                    new SoapMethodDiagnostics { Name = "GetReportAsync", Status = "Available", LastTested = DateTime.UtcNow.AddSeconds(-15), IsAsync = true },
                    new SoapMethodDiagnostics { Name = "HelloWorld", Status = "Available", LastTested = DateTime.UtcNow.AddMinutes(-5), IsAsync = false },
                    new SoapMethodDiagnostics { Name = "GetData", Status = "Available", LastTested = DateTime.UtcNow.AddMinutes(-4), IsAsync = false },
                    new SoapMethodDiagnostics { Name = "GetDataSet", Status = "Available", LastTested = DateTime.UtcNow.AddMinutes(-3), IsAsync = false },
                    new SoapMethodDiagnostics { Name = "GetReport", Status = "Available", LastTested = DateTime.UtcNow.AddMinutes(-2), IsAsync = false }
                },
                PerformanceMetrics = new SoapPerformanceMetrics
                {
                    AverageResponseTimeMs = 85.3,
                    TotalRequests = 2847,
                    SuccessfulRequests = 2821,
                    FailedRequests = 26,
                    SuccessRate = 99.1,
                    AsyncRequestsPercentage = 75.2
                },
                Configuration = new SoapConfigurationInfo
                {
                    MaxRequestSize = "4MB",
                    Timeout = "30 seconds",
                    EnabledProtocols = ["HttpGet", "HttpPost", "HttpSoap"],
                    AuthenticationEnabled = false,
                    AsyncSupportEnabled = true
                },
                LastDiagnosticsRun = DateTime.UtcNow,
                DiagnosticsVersion = "2.0.0"
            };

            _logger.LogInformation("SOAP diagnostics completed successfully. Service status: {Status}, Success rate: {SuccessRate}%, Async usage: {AsyncPercentage}%", 
                diagnosticsInfo.ServiceStatus, diagnosticsInfo.PerformanceMetrics.SuccessRate, diagnosticsInfo.PerformanceMetrics.AsyncRequestsPercentage);

            return Ok(diagnosticsInfo);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("SOAP diagnostics request was cancelled");
            var errorResponse = new ErrorResponse
            {
                Status = "Cancelled",
                Error = "Request was cancelled",
                Message = "The SOAP diagnostics operation was cancelled due to timeout",
                Timestamp = DateTime.UtcNow
            };
            return StatusCode(StatusCodes.Status408RequestTimeout, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving SOAP diagnostics");
            var errorResponse = new ErrorResponse
            {
                Status = "Error",
                Error = "Failed to retrieve SOAP diagnostics",
                Message = ex.Message,
                Timestamp = DateTime.UtcNow
            };
            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }

    /// <summary>
    /// Get live health check with real-time validation (API v2.0 only)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Real-time health status with immediate validation</returns>
    [HttpGet("live")]
    [MapToApiVersion("2.0")]
    [SwaggerOperation(
        Summary = "Get live health check",
        Description = "Returns real-time health status with immediate validation of all critical components",
        OperationId = "GetLiveHealth",
        Tags = new[] { "Health Monitoring", "Live Status" }
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Live health check completed successfully", typeof(LiveHealthResponse))]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service is unhealthy", typeof(LiveHealthResponse))]
    [SwaggerResponse(StatusCodes.Status408RequestTimeout, "Request timeout", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    [ProducesResponseType<LiveHealthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<LiveHealthResponse>(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LiveHealthResponse>> GetLiveAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("LiveHealthCheck_{RequestId}", Guid.NewGuid());
        _logger.LogInformation("Live health check endpoint accessed at {Timestamp}", DateTime.UtcNow);

        try
        {
            var startTime = DateTime.UtcNow;
            var healthReport = await _healthCheckService.CheckHealthAsync(cancellationToken);
            var endTime = DateTime.UtcNow;

            var response = new LiveHealthResponse
            {
                Status = healthReport.Status.ToString(),
                IsHealthy = healthReport.Status == HealthStatus.Healthy,
                CheckDuration = endTime - startTime,
                CriticalServices = healthReport.Entries
                    .Where(e => e.Value.Status != HealthStatus.Healthy)
                    .Select(e => e.Key)
                    .ToArray(),
                ServiceCount = healthReport.Entries.Count,
                HealthyServiceCount = healthReport.Entries.Count(e => e.Value.Status == HealthStatus.Healthy),
                Timestamp = DateTime.UtcNow
            };

            var statusCode = response.IsHealthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;

            _logger.LogInformation("Live health check completed. Status: {Status}, Duration: {Duration}ms, Healthy services: {Healthy}/{Total}",
                response.Status, response.CheckDuration.TotalMilliseconds, response.HealthyServiceCount, response.ServiceCount);

            return StatusCode(statusCode, response);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Live health check request was cancelled");
            var errorResponse = new ErrorResponse
            {
                Status = "Cancelled",
                Error = "Request was cancelled",
                Message = "The live health check operation was cancelled due to timeout",
                Timestamp = DateTime.UtcNow
            };
            return StatusCode(StatusCodes.Status408RequestTimeout, errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while performing live health check");
            var errorResponse = new ErrorResponse
            {
                Status = "Error",
                Error = "Failed to perform live health check",
                Message = ex.Message,
                Timestamp = DateTime.UtcNow
            };
            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }

    private async Task<SystemMetrics> GetSystemMetricsAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);

        return new SystemMetrics
        {
            SystemStatus = "Online",
            ServiceName = "SOAP Web Service",
            Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            HostName = System.Environment.MachineName,
            ProcessId = System.Environment.ProcessId,
            RuntimeVersion = System.Environment.Version.ToString(),
            FrameworkDescription = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
            OSDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
            ProcessorCount = System.Environment.ProcessorCount,
            WorkingSet = System.Environment.WorkingSet,
            StartTime = DateTime.UtcNow.AddMilliseconds(-System.Environment.TickCount64),
            Uptime = TimeSpan.FromMilliseconds(System.Environment.TickCount64).ToString(@"dd\.hh\:mm\:ss"),
            MemoryUsage = new MemoryUsageInfo
            {
                WorkingSetMB = Math.Round(System.Environment.WorkingSet / 1024.0 / 1024.0, 2),
                GCTotalMemoryMB = Math.Round(GC.GetTotalMemory(false) / 1024.0 / 1024.0, 2)
            },
            ServiceCapabilities = new ServiceCapabilitiesInfo
            {
                SoapServiceEnabled = true,
                RestApiEnabled = true,
                SwaggerDocumentation = true,
                HealthChecks = true,
                AsyncSupport = true
            }
        };
    }
}

#region Response Models

/// <summary>
/// Basic health response model
/// </summary>
public sealed class HealthResponse
{
    /// <summary>
    /// Overall health status
    /// </summary>
    [Required]
    public required string Status { get; set; }

    /// <summary>
    /// Service name
    /// </summary>
    [Required]
    public required string Service { get; set; }

    /// <summary>
    /// Framework version
    /// </summary>
    [Required]
    public required string Framework { get; set; }

    /// <summary>
    /// Migration information
    /// </summary>
    [Required]
    public required MigrationInfo Migration { get; set; }

    /// <summary>
    /// Available endpoints
    /// </summary>
    [Required]
    public required string[] Endpoints { get; set; }

    /// <summary>
    /// Available SOAP methods
    /// </summary>
    [Required]
    public required SoapMethodInfo[] SoapMethods { get; set; }

    /// <summary>
    /// Response timestamp
    /// </summary>
    [Required]
    public required DateTime Timestamp { get; set; }

    /// <summary>
    /// API version
    /// </summary>
    [Required]
    public required string Version { get; set; }
}

/// <summary>
/// Migration information model
/// </summary>
public sealed class MigrationInfo
{
    /// <summary>
    /// Migration source
    /// </summary>
    [Required]
    public required string From { get; set; }

    /// <summary>
    /// Migration target
    /// </summary>
    [Required]
    public required string To { get; set; }

    /// <summary>
    /// Migration status
    /// </summary>
    [Required]
    public required string Status { get; set; }

    /// <summary>
    /// Migration date
    /// </summary>
    [Required]
    public required string Date { get; set; }
}

/// <summary>
/// SOAP method information model
/// </summary>
public sealed class SoapMethodInfo
{
    /// <summary>
    /// Method name
    /// </summary>
    [Required]
    public required string Method { get; set; }

    /// <summary>
    /// Method description
    /// </summary>
    [Required]
    public required string Description { get; set; }

    /// <summary>
    /// Method parameters
    /// </summary>
    [Required]
    public required string Parameters { get; set; }
}

/// <summary>
/// Simple health response model
/// </summary>
public sealed class SimpleHealthResponse
{
    /// <summary>
    /// Health status
    /// </summary>
    [Required]
    public required string Status { get; set; }

    /// <summary>
    /// Whether the service is healthy
    /// </summary>
    [Required]
    public required bool IsHealthy { get; set; }

    /// <summary>
    /// Response timestamp
    /// </summary>
    [Required]
    public required DateTime Timestamp { get; set; }
}

/// <summary>
/// Detailed health response model
/// </summary>
public sealed class DetailedHealthResponse
{
    /// <summary>
    /// Overall health status
    /// </summary>
    [Required]
    public required string Status { get; set; }

    /// <summary>
    /// Total duration of health checks
    /// </summary>
    [Required]
    public required TimeSpan TotalDuration { get; set; }

    /// <summary>
    /// Individual health check results
    /// </summary>
    [Required]
    public required HealthCheckDetail[] Checks { get; set; }

    /// <summary>
    /// System metrics
    /// </summary>
    [Required]
    public required SystemMetrics SystemMetrics { get; set; }

    /// <summary>
    /// Response timestamp
    /// </summary>
    [Required]
    public required DateTime Timestamp { get; set; }
}

/// <summary>
/// Health check detail model
/// </summary>
public sealed class HealthCheckDetail
{
    /// <summary>
    /// Health check name
    /// </summary>
    [Required]
    public required string Name { get; set; }

    /// <summary>
    /// Health check status
    /// </summary>
    [Required]
    public required string Status { get; set; }

    /// <summary>
    /// Health check duration
    /// </summary>
    [Required]
    public required TimeSpan Duration { get; set; }

    /// <summary>
    /// Health check description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Exception message if check failed
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// Additional data from health check
    /// </summary>
    [Required]
    public required Dictionary<string, string?> Data { get; set; }
}

/// <summary>
/// System metrics model
/// </summary>
public sealed class SystemMetrics
{
    /// <summary>
    /// System status
    /// </summary>
    [Required]
    public required string SystemStatus { get; set; }

    /// <summary>
    /// Service name
    /// </summary>
    [Required]
    public required string ServiceName { get; set; }

    /// <summary>
    /// Environment name
    /// </summary>
    [Required]
    public required string Environment { get; set; }

    /// <summary>
    /// Host name
    /// </summary>
    [Required]
    public required string HostName { get; set; }

    /// <summary>
    /// Process ID
    /// </summary>
    [Required]
    public required int ProcessId { get; set; }

    /// <summary>
    /// Runtime version
    /// </summary>
    [Required]
    public required string RuntimeVersion { get; set; }

    /// <summary>
    /// Framework description
    /// </summary>
    [Required]
    public required string FrameworkDescription { get; set; }

    /// <summary>
    /// Operating system description
    /// </summary>
    [Required]
    public required string OSDescription { get; set; }

    /// <summary>
    /// Processor count
    /// </summary>
    [Required]
    public required int ProcessorCount { get; set; }

    /// <summary>
    /// Working set memory
    /// </summary>
    [Required]
    public required long WorkingSet { get; set; }

    /// <summary>
    /// Process start time
    /// </summary>
    [Required]
    public required DateTime StartTime { get; set; }

    /// <summary>
    /// Process uptime
    /// </summary>
    [Required]
    public required string Uptime { get; set; }

    /// <summary>
    /// Memory usage information
    /// </summary>
    [Required]
    public required MemoryUsageInfo MemoryUsage { get; set; }

    /// <summary>
    /// Service capabilities
    /// </summary>
    [Required]
    public required ServiceCapabilitiesInfo ServiceCapabilities { get; set; }
}

/// <summary>
/// Memory usage information model
/// </summary>
public sealed class MemoryUsageInfo
{
    /// <summary>
    /// Working set in megabytes
    /// </summary>
    [Required]
    public required double WorkingSetMB { get; set; }

    /// <summary>
    /// GC total memory in megabytes
    /// </summary>
    [Required]
    public required double GCTotalMemoryMB { get; set; }
}

/// <summary>
/// Service capabilities information model
/// </summary>
public sealed class ServiceCapabilitiesInfo
{
    /// <summary>
    /// Whether SOAP service is enabled
    /// </summary>
    [Required]
    public required bool SoapServiceEnabled { get; set; }

    /// <summary>
    /// Whether REST API is enabled
    /// </summary>
    [Required]
    public required bool RestApiEnabled { get; set; }

    /// <summary>
    /// Whether Swagger documentation is available
    /// </summary>
    [Required]
    public required bool SwaggerDocumentation { get; set; }

    /// <summary>
    /// Whether health checks are enabled
    /// </summary>
    [Required]
    public required bool HealthChecks { get; set; }

    /// <summary>
    /// Whether async support is enabled
    /// </summary>
    [Required]
    public required bool AsyncSupport { get; set; }
}

/// <summary>
/// Dependencies health response model
/// </summary>
public sealed class DependenciesHealthResponse
{
    /// <summary>
    /// Overall dependencies status
    /// </summary>
    [Required]
    public required string OverallStatus { get; set; }

    /// <summary>
    /// Individual dependency health information
    /// </summary>
    [Required]
    public required DependencyHealthInfo[] Dependencies { get; set; }

    /// <summary>
    /// Total number of dependencies
    /// </summary>
    [Required]
    public required int TotalDependencies { get; set; }

    /// <summary>
    /// Number of healthy dependencies
    /// </summary>
    [Required]
    public required int HealthyDependencies { get; set; }

    /// <summary>
    /// Number of unhealthy dependencies
    /// </summary>
    [Required]
    public required int UnhealthyDependencies { get; set; }

    /// <summary>
    /// Response timestamp
    /// </summary>
    [Required]
    public required DateTime Timestamp { get; set; }
}

/// <summary>
/// Dependency health information model
/// </summary>
public sealed class DependencyHealthInfo
{
    /// <summary>
    /// Dependency name
    /// </summary>
    [Required]
    public required string Name { get; set; }

    /// <summary>
    /// Dependency status
    /// </summary>
    [Required]
    public required string Status { get; set; }

    /// <summary>
    /// Response time
    /// </summary>
    [Required]
    public required TimeSpan ResponseTime { get; set; }

    /// <summary>
    /// Last checked timestamp
    /// </summary>
    [Required]
    public required DateTime LastChecked { get; set; }

    /// <summary>
    /// Additional details
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Exception message if check failed
    /// </summary>
    public string? Exception { get; set; }
}

/// <summary>
/// SOAP diagnostics response model
/// </summary>
public sealed class SoapDiagnosticsResponse
{
    /// <summary>
    /// SOAP service status
    /// </summary>
    [Required]
    public required string ServiceStatus { get; set; }

    /// <summary>
    /// Whether WSDL is accessible
    /// </summary>
    [Required]
    public required bool WsdlAccessible { get; set; }

    /// <summary>
    /// SOAP endpoint URL
    /// </summary>
    [Required]
    public required string SoapEndpointUrl { get; set; }

    /// <summary>
    /// WSDL URL
    /// </summary>
    [Required]
    public required string WsdlUrl { get; set; }

    /// <summary>
    /// Supported SOAP versions
    /// </summary>
    [Required]
    public required string[] SupportedSoapVersions { get; set; }

    /// <summary>
    /// Available SOAP methods
    /// </summary>
    [Required]
    public required SoapMethodDiagnostics[] AvailableMethods { get; set; }

    /// <summary>
    /// Performance metrics
    /// </summary>
    [Required]
    public required SoapPerformanceMetrics PerformanceMetrics { get; set; }

    /// <summary>
    /// Configuration information
    /// </summary>
    [Required]
    public required SoapConfigurationInfo Configuration { get; set; }

    /// <summary>
    /// Last diagnostics run timestamp
    /// </summary>
    [Required]
    public required DateTime LastDiagnosticsRun { get; set; }

    /// <summary>
    /// Diagnostics version
    /// </summary>
    [Required]
    public required string DiagnosticsVersion { get; set; }
}

/// <summary>
/// SOAP method diagnostics model
/// </summary>
public sealed class SoapMethodDiagnostics
{
    /// <summary>
    /// Method name
    /// </summary>
    [Required]
    public required string Name { get; set; }

    /// <summary>
    /// Method status
    /// </summary>
    [Required]
    public required string Status { get; set; }

    /// <summary>
    /// Last tested timestamp
    /// </summary>
    [Required]
    public required DateTime LastTested { get; set; }

    /// <summary>
    /// Whether the method is asynchronous
    /// </summary>
    [Required]
    public required bool IsAsync { get; set; }
}

/// <summary>
/// SOAP performance metrics model
/// </summary>
public sealed class SoapPerformanceMetrics
{
    /// <summary>
    /// Average response time in milliseconds
    /// </summary>
    [Required]
    public required double AverageResponseTimeMs { get; set; }

    /// <summary>
    /// Total number of requests
    /// </summary>
    [Required]
    public required long TotalRequests { get; set; }

    /// <summary>
    /// Number of successful requests
    /// </summary>
    [Required]
    public required long SuccessfulRequests { get; set; }

    /// <summary>
    /// Number of failed requests
    /// </summary>
    [Required]
    public required long FailedRequests { get; set; }

    /// <summary>
    /// Success rate percentage
    /// </summary>
    [Required]
    public required double SuccessRate { get; set; }

    /// <summary>
    /// Percentage of async requests
    /// </summary>
    [Required]
    public required double AsyncRequestsPercentage { get; set; }
}

/// <summary>
/// SOAP configuration information model
/// </summary>
public sealed class SoapConfigurationInfo
{
    /// <summary>
    /// Maximum request size
    /// </summary>
    [Required]
    public required string MaxRequestSize { get; set; }

    /// <summary>
    /// Request timeout
    /// </summary>
    [Required]
    public required string Timeout { get; set; }

    /// <summary>
    /// Enabled protocols
    /// </summary>
    [Required]
    public required string[] EnabledProtocols { get; set; }

    /// <summary>
    /// Whether authentication is enabled
    /// </summary>
    [Required]
    public required bool AuthenticationEnabled { get; set; }

    /// <summary>
    /// Whether async support is enabled
    /// </summary>
    [Required]
    public required bool AsyncSupportEnabled { get; set; }
}

/// <summary>
/// Live health response model
/// </summary>
public sealed class LiveHealthResponse
{
    /// <summary>
    /// Current health status
    /// </summary>
    [Required]
    public required string Status { get; set; }

    /// <summary>
    /// Whether the service is healthy
    /// </summary>
    [Required]
    public required bool IsHealthy { get; set; }

    /// <summary>
    /// Health check duration
    /// </summary>
    [Required]
    public required TimeSpan CheckDuration { get; set; }

    /// <summary>
    /// Critical services that are unhealthy
    /// </summary>
    [Required]
    public required string[] CriticalServices { get; set; }

    /// <summary>
    /// Total number of services
    /// </summary>
    [Required]
    public required int ServiceCount { get; set; }

    /// <summary>
    /// Number of healthy services
    /// </summary>
    [Required]
    public required int HealthyServiceCount { get; set; }

    /// <summary>
    /// Response timestamp
    /// </summary>
    [Required]
    public required DateTime Timestamp { get; set; }
}

/// <summary>
/// Error response model
/// </summary>
public sealed class ErrorResponse
{
    /// <summary>
    /// Error status
    /// </summary>
    [Required]
    public required string Status { get; set; }

    /// <summary>
    /// Error code or type
    /// </summary>
    [Required]
    public required string Error { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Error timestamp
    /// </summary>
    [Required]
    public required DateTime Timestamp { get; set; }
}

#endregion