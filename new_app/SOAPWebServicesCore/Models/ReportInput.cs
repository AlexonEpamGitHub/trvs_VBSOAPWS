namespace SOAPWebServicesCore.Models
{
    /// <summary>
    /// Represents the input parameters for report generation.
    /// </summary>
    public class ReportInput
    {
        /// <summary>
        /// Gets or sets the name of the report to be generated.
        /// </summary>
        public string ReportName { get; set; } = string.Empty;
    }
}