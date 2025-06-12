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
                Endpoints = new[]
                {
                    "/GetDataService.asmx - SOAP Service",
                    "/GetDataService.asmx?wsdl - WSDL Definition"
                }
            });
        }
    }
}