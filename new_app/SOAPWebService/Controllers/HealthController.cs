using Microsoft.AspNetCore.Mvc;

namespace SOAPWebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Health check endpoint accessed at {Timestamp}", DateTime.UtcNow);

            try
            {
                var healthInfo = new
                {
                    Status = "Healthy",
                    Service = "SOAP Web Service",
                    Framework = ".NET 8.0",
                    Migration = new
                    {
                        From = "Legacy SOAP Service",
                        To = "Modern .NET 8.0 Implementation",
                        Status = "Completed",
                        Date = DateTime.UtcNow.ToString("yyyy-MM-dd")
                    },
                    Endpoints = new[]
                    {
                        "/GetDataService.asmx - SOAP Service",
                        "/GetDataService.asmx?wsdl - WSDL Definition",
                        "/api/health - REST Health Check",
                        "/api/health/status - System Status Information",
                        "/swagger - API Documentation"
                    },
                    SoapMethods = new[]
                    {
                        new { Method = "HelloWorld", Description = "Simple hello world test method", Parameters = "None" },
                        new { Method = "GetData", Description = "Retrieves basic data", Parameters = "None" },
                        new { Method = "GetDataSet", Description = "Retrieves data in DataSet format", Parameters = "None" },
                        new { Method = "GetReport", Description = "Generates and returns report data", Parameters = "None" }
                    },
                    Timestamp = DateTime.UtcNow,
                    Version = "1.0.0"
                };

                _logger.LogInformation("Health check completed successfully");
                return Ok(healthInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing health check request");
                return StatusCode(500, new
                {
                    Status = "Unhealthy",
                    Error = "Internal server error occurred",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            _logger.LogInformation("System status endpoint accessed at {Timestamp}", DateTime.UtcNow);

            try
            {
                var systemInfo = new
                {
                    SystemStatus = "Online",
                    ServiceName = "SOAP Web Service",
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    HostName = Environment.MachineName,
                    ProcessId = Environment.ProcessId,
                    RuntimeVersion = Environment.Version.ToString(),
                    FrameworkDescription = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                    OSDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                    ProcessorCount = Environment.ProcessorCount,
                    WorkingSet = Environment.WorkingSet,
                    StartTime = DateTime.UtcNow.AddMilliseconds(-Environment.TickCount64),
                    Uptime = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(@"dd\.hh\:mm\:ss"),
                    MemoryUsage = new
                    {
                        WorkingSetMB = Math.Round(Environment.WorkingSet / 1024.0 / 1024.0, 2),
                        GCTotalMemoryMB = Math.Round(GC.GetTotalMemory(false) / 1024.0 / 1024.0, 2)
                    },
                    ServiceCapabilities = new
                    {
                        SoapServiceEnabled = true,
                        RestApiEnabled = true,
                        SwaggerDocumentation = true,
                        HealthChecks = true
                    },
                    LastChecked = DateTime.UtcNow,
                    Version = "1.0.0"
                };

                _logger.LogInformation("System status retrieved successfully");
                return Ok(systemInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving system status");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Unable to retrieve system status",
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
}