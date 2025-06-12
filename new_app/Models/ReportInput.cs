using System.Runtime.Serialization;

namespace SOAPWebServicesCore.Models;

/// <summary>
/// Data transfer object for report input parameters used in SOAP web services.
/// This class replaces the legacy cReportInput.vb Visual Basic class with modern C# conventions
/// and provides full compatibility with SOAP serialization for web service operations.
/// </summary>
/// <remarks>
/// This class is designed to be serialized and deserialized by SOAP services and ensures
/// proper data contract compliance for cross-platform web service communication.
/// The implementation follows .NET 8 coding standards and conventions.
/// </remarks>
[DataContract(Name = "ReportInput", Namespace = "http://soapwebservicescore.models")]
public class ReportInput
{
    /// <summary>
    /// Gets or sets the name of the report to be generated.
    /// </summary>
    /// <value>
    /// A string representing the report name. This property is required for report generation
    /// and defaults to an empty string if not specified. The value should correspond to a
    /// valid report identifier in the reporting system.
    /// </value>
    /// <example>
    /// Example usage:
    /// <code>
    /// var reportInput = new ReportInput { ReportName = "MonthlyReport" };
    /// </code>
    /// </example>
    [DataMember(Name = "ReportName", IsRequired = true, Order = 1)]
    public string ReportName { get; set; } = string.Empty;
}
