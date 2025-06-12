using System.Runtime.Serialization;

namespace SOAPWebServicesCore.Models;

[DataContract(Namespace = "http://tempuri.org/")]
public class ReportInput
{
    [DataMember]
    public string ReportName { get; set; } = string.Empty;
}