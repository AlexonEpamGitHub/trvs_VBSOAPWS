using SOAPWebServicesSimpleCore.Models;
using System.Data;
using System.ServiceModel;

namespace SOAPWebServicesSimpleCore.Interfaces
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