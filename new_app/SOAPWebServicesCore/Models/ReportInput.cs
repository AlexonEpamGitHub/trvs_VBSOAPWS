using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SOAPWebServicesCore.Models
{
    [DataContract(Namespace = "http://tempuri.org/")]
    [XmlRoot("cReportInput", Namespace = "http://tempuri.org/")]
    public class ReportInput
    {
        [DataMember]
        [XmlElement("ReportName")]
        public string? ReportName { get; set; }
    }
}
