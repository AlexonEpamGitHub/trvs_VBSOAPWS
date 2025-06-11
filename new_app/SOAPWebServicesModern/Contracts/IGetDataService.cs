using System.Data;
using System.ServiceModel;

namespace SOAPWebServicesModern.Contracts
{
    [ServiceContract(Namespace = "http://tempuri.org/")]
    public interface IGetDataService
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