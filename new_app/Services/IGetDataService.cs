using CoreWCF;
using SOAPWebServicesCore.Models;
using System.Data;

namespace SOAPWebServicesCore.Services
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