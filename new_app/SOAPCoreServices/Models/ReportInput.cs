using System;
using System.Runtime.Serialization;

namespace SOAPCoreServices.Models
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