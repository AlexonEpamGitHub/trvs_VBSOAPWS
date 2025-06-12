namespace SOAPWebServicesSimpleCore.Models
{
    /// <summary>
    /// Represents input data for report generation.
    /// </summary>
    public sealed class ReportInput
    {
        /// <summary>
        /// Gets or sets the name of the report to be generated.
        /// </summary>
        /// <remarks>
        /// Can be null if a default report should be used.
        /// </remarks>
        public required string? ReportName { get; set; }
    }
}