using System.Runtime.Serialization;

namespace SOAPWebService.Models
{
    /// <summary>
    /// Data transfer object for report input parameters.
    /// Migrated from legacy VB.NET cReportInput class to modern C# with DataContract serialization.
    /// </summary>
    [DataContract]
    public class ReportInput
    {
        /// <summary>
        /// Gets or sets the name of the report to generate.
        /// </summary>
        [DataMember]
        public string ReportName { get; set; } = string.Empty;
    }
}