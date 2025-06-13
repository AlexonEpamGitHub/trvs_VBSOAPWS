using System.Data;
using System.Runtime.Serialization;
using System.ServiceModel;
using SOAPWebServicesSimple.Models;

namespace SOAPWebServicesSimple.Services
{
    [ServiceContract(Namespace = "http://tempuri.org/")]
    public interface IDataService
    {
        [OperationContract]
        string HelloWorld();

        [OperationContract]
        string GetData(string name);

        [OperationContract]
        DataSet GetDataSet();

        [OperationContract]
        DataSet GetReport(ReportInput reportInput);
    }
}