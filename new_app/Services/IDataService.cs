using System.Data;
using System.ServiceModel;
using SOAPWebServicesCore.Models;

namespace SOAPWebServicesCore.Services;

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