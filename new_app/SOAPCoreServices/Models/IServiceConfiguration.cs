namespace SOAPCoreServices.Models
{
    /// <summary>
    /// Interface for service configuration settings
    /// </summary>
    public interface IServiceConfiguration
    {
        /// <summary>
        /// Gets the maximum size of received messages
        /// </summary>
        int MaxReceivedMessageSize { get; }

        /// <summary>
        /// Gets the maximum array length for serialization
        /// </summary>
        int MaxArrayLength { get; }

        /// <summary>
        /// Gets the maximum string content length
        /// </summary>
        int MaxStringContentLength { get; }
    }
}