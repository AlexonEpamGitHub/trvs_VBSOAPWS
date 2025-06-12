using System.Data;
using System.ServiceModel;
using SOAPWebService.Models;

namespace SOAPWebService.Services
{
    [ServiceContract]
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