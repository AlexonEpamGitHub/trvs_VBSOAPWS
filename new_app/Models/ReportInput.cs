using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SOAPWebServicesCore.Models;

/// <summary>
/// Represents the input parameters for generating reports in the SOAP web service.
/// This class is designed to be compatible with legacy VB.NET cReportInput structure
/// and provides proper SOAP serialization support for SoapCore framework.
/// </summary>
/// <remarks>
/// This class maintains backward compatibility with existing SOAP clients and uses
/// the exact namespace 'http://tempuri.org/' to match the original VB.NET service.
/// It supports both DataContract and XML serialization for maximum compatibility.
/// The XmlRoot attribute ensures proper SOAP envelope serialization, while DataContract
/// provides WCF compatibility. The class follows .NET 8 coding standards with proper
/// null safety and string handling.
/// </remarks>
[DataContract(Namespace = "http://tempuri.org/")]
[XmlRoot("cReportInput", Namespace = "http://tempuri.org/")]
public class ReportInput
{
    /// <summary>
    /// Gets or sets the name of the report to be generated.
    /// This property specifies which report template or type should be used
    /// for report generation and must match the available report templates.
    /// </summary>
    /// <value>
    /// A string representing the report name. Default value is an empty string.
    /// This property is required for proper report generation and supports null values
    /// for backward compatibility with legacy clients.
    /// </value>
    /// <example>
    /// Example usage:
    /// <code>
    /// var reportInput = new ReportInput 
    /// { 
    ///     ReportName = "MonthlyReport" 
    /// };
    /// </code>
    /// </example>
    /// <remarks>
    /// The XmlElement attribute ensures proper XML serialization for SOAP clients,
    /// while the DataMember attribute provides WCF/DataContract serialization support.
    /// The Order parameter ensures consistent serialization order across different
    /// serialization mechanisms.
    /// </remarks>
    [DataMember(Name = "ReportName", Order = 1)]
    [XmlElement("ReportName")]
    public string ReportName { get; set; } = string.Empty;
}