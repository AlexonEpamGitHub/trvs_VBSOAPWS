using System.Runtime.Serialization;

namespace SOAPWebServicesSimple.Models
{
    [DataContract]
    public class ReportInput
    {
        private string _reportName;

        [DataMember]
        public string ReportName
        {
            get { return _reportName; }
            set { _reportName = value; }
        }
    }
}