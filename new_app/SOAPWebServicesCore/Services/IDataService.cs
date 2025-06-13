using SOAPWebServicesCore.Models;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Web;

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
}
