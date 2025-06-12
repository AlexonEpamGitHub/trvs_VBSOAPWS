using Microsoft.AspNetCore.Mvc;

namespace SOAPWebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
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
                    "/swagger - API Documentation"
                },
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0"
            });
        }
    }
}