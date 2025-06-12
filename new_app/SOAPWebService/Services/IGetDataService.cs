using System.Data;
using System.ServiceModel;
using SOAPWebService.Models;

namespace SOAPWebService.Services
{
    /// <summary>
    /// Service contract interface for SOAP web service operations.
    /// Migrated from legacy VB.NET WebService to modern .NET 8 ServiceContract.
    /// </summary>
    [ServiceContract(Namespace = "http://tempuri.org/")]
    public interface IGetDataService
    {
        /// <summary>
        /// Returns a simple "Hello World" greeting.
        /// </summary>
        /// <returns>Hello World string</returns>
        [OperationContract(Action = "http://tempuri.org/HelloWorld", ReplyAction = "http://tempuri.org/HelloWorldResponse")]
        string HelloWorld();

        /// <summary>
        /// Returns a personalized greeting message.
        /// </summary>
        /// <param name="name">Name to include in the greeting</param>
        /// <returns>Personalized greeting string</returns>
        [OperationContract(Action = "http://tempuri.org/GetData", ReplyAction = "http://tempuri.org/GetDataResponse")]
        string GetData(string name);

        /// <summary>
        /// Returns a sample DataSet with test data.
        /// </summary>
        /// <returns>DataSet containing sample data</returns>
        [OperationContract(Action = "http://tempuri.org/GetDataSet", ReplyAction = "http://tempuri.org/GetDataSetResponse")]
        DataSet GetDataSet();

        /// <summary>
        /// Returns report data based on the provided input parameters.
        /// </summary>
        /// <param name="reportInput">Report input parameters</param>
        /// <returns>DataSet containing report data</returns>
        [OperationContract(Action = "http://tempuri.org/GetReport", ReplyAction = "http://tempuri.org/GetReportResponse")]
        DataSet GetReport(ReportInput reportInput);
    }
}