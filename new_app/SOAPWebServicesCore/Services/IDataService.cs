using SOAPWebServicesCore.Models;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SOAPWebServicesCore.Services
{
    [ServiceContract(Namespace = "http://tempuri.org/")]
    [XmlSerializerFormat(SupportFaults = true)]
    public interface IDataService
    {
        [OperationContract(Action = "http://tempuri.org/HelloWorld")]
        string HelloWorld();
        
        [OperationContract(Action = "http://tempuri.org/GetData")]
        string GetData(string name);
        
        [OperationContract(Action = "http://tempuri.org/GetDataSet")]
        DataSet GetDataSet();
        
        [OperationContract(Action = "http://tempuri.org/GetReport")]
        DataSet GetReport(ReportInput reportInput);
    }

    [MessageContract(IsWrapped = true, WrapperName = "ReportRequest", WrapperNamespace = "http://tempuri.org/")]
    public class ReportRequest
    {
        [MessageBodyMember(Namespace = "http://tempuri.org/")]
        public ReportInput ReportInput { get; set; }
    }

    [MessageContract(IsWrapped = true, WrapperName = "ReportResponse", WrapperNamespace = "http://tempuri.org/")]
    public class ReportResponse
    {
        [MessageBodyMember(Namespace = "http://tempuri.org/")]
        public DataSet ReportResult { get; set; }
    }

    [MessageContract(IsWrapped = true, WrapperName = "GetDataRequest", WrapperNamespace = "http://tempuri.org/")]
    public class GetDataRequest
    {
        [MessageBodyMember(Namespace = "http://tempuri.org/")]
        public string Name { get; set; }
    }

    [MessageContract(IsWrapped = true, WrapperName = "GetDataResponse", WrapperNamespace = "http://tempuri.org/")]
    public class GetDataResponse
    {
        [MessageBodyMember(Namespace = "http://tempuri.org/")]
        public string Result { get; set; }
    }
}
