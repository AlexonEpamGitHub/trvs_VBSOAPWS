using System.Runtime.Serialization;

namespace SOAPWebServicesCore.Models;

/// <summary>
/// Data transfer object for report input parameters used in SOAP web services.
/// This class replaces the legacy cReportInput.vb Visual Basic class with modern C# conventions.
/// </summary>
[DataContract]
public class ReportInput
{
    /// <summary>
    /// Gets or sets the name of the report to be generated.
    /// </summary>
    /// <value>
    /// A string representing the report name. Defaults to an empty string if not specified.
    /// </value>
    [DataMember]
    public string ReportName { get; set; } = string.Empty;
}