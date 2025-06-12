using System.Runtime.Serialization;

namespace SOAPWebServicesCore.Models;

[DataContract]
public class ReportInput
{
    [DataMember]
    public string ReportName { get; set; }
}