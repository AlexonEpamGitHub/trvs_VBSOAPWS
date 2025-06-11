using System.ServiceModel;
using System.Data;
using SOAPWebServices.Core.Models;

namespace SOAPWebServices.Core.Contracts
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