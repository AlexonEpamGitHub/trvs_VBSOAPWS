using SOAPWebServicesCore.Models;
using System.Data;
using System.ServiceModel;

namespace SOAPWebServicesCore.Services
{
    /// <summary>
    /// Interface for the SOAP web service that exposes data operations
    /// </summary>
    [ServiceContract(Namespace = "http://tempuri.org/")]
    public interface IGetDataService
    {
        /// <summary>
        /// A simple hello world method
        /// </summary>
        /// <returns>A greeting string</returns>
        [OperationContract]
        string HelloWorld();

        /// <summary>
        /// Returns a personalized greeting
        /// </summary>
        /// <param name="name">The name to personalize the greeting</param>
        /// <returns>A personalized greeting string</returns>
        [OperationContract]
        string GetData(string name);

        /// <summary>
        /// Returns a sample dataset
        /// </summary>
        /// <returns>A dataset with sample data</returns>
        [OperationContract]
        DataSet GetDataSet();

        /// <summary>
        /// Returns report data based on the provided report input
        /// </summary>
        /// <param name="reportInput">The report input parameters</param>
        /// <returns>A dataset containing the report data</returns>
        [OperationContract]
        DataSet GetReport(ref ReportInput reportInput);
    }
}
