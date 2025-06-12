using System.Runtime.Serialization;

namespace SOAPWebServicesCore.Models
{
    /// <summary>
    /// Represents the input parameters for generating reports in the SOAP web service.
    /// This class is designed to be compatible with legacy VB.NET cReportInput structure
    /// and provides proper SOAP serialization support.
    /// </summary>
    [DataContract(Namespace = "http://tempuri.org/")]
    public class ReportInput
    {
        /// <summary>
        /// Gets or sets the name of the report to be generated.
        /// This property specifies which report template or type should be used
        /// for report generation.
        /// </summary>
        /// <value>
        /// A string representing the report name. Default value is an empty string.
        /// </value>
        [DataMember]
        public string ReportName { get; set; } = string.Empty;
    }
}