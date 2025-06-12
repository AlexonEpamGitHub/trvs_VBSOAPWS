using System.Runtime.Serialization;

namespace SOAPWebService.Models
{
    [DataContract]
    public class ReportInput
    {
        [DataMember]
        public string ReportName { get; set; } = string.Empty;
    }
}