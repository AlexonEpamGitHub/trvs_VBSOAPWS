using SOAPWebServicesSimple.Models;
using System.Data;
using System.ServiceModel;

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
        DataSet GetReport(ref ReportInput reportInput);
    }
}