using System.Runtime.Serialization;

namespace SOAPWebService.Models
{
    /// <summary>
    /// Represents input data for generating reports in the SOAP web service.
    /// This class serves as a data transfer object (DTO) for report-related operations
    /// and replaces the legacy cReportInput VB.NET class.
    /// </summary>
    /// <remarks>
    /// This class is decorated with DataContract attributes to ensure proper SOAP serialization
    /// and follows .NET 8 coding standards including nullable reference type conventions.
    /// </remarks>
    [DataContract]
    public class ReportInput
    {
        /// <summary>
        /// Gets or sets the name of the report to be generated.
        /// </summary>
        /// <value>
        /// A string representing the report name. Defaults to an empty string.
        /// </value>
        /// <remarks>
        /// This property is marked with DataMember attribute to ensure it is included
        /// in SOAP serialization operations.
        /// </remarks>
        [DataMember]
        public string ReportName { get; set; } = string.Empty;
    }
}