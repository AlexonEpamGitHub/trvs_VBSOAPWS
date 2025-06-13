using System;
using System.Data;
using System.ServiceModel;
using System.Threading.Tasks;

namespace SOAPCoreServices.Interfaces
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
        DataSet GetReport(Models.ReportInput reportInput);
    }
}