using Microsoft.Extensions.Configuration;

namespace SOAPCoreServices.Models
{
    /// <summary>
    /// Implementation of service configuration settings
    /// </summary>
    public class ServiceConfiguration : IServiceConfiguration
    {
        private readonly IConfiguration _configuration;

        public ServiceConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Gets the maximum size of received messages
        /// </summary>
        public int MaxReceivedMessageSize => _configuration.GetValue<int>("SoapService:MaxReceivedMessageSize", 1048576);

        /// <summary>
        /// Gets the maximum array length for serialization
        /// </summary>
        public int MaxArrayLength => _configuration.GetValue<int>("SoapService:MaxArrayLength", 1048576);

        /// <summary>
        /// Gets the maximum string content length
        /// </summary>
        public int MaxStringContentLength => _configuration.GetValue<int>("SoapService:MaxStringContentLength", 1048576);
    }
}