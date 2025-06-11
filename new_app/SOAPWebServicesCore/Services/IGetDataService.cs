using SOAPWebServicesCore.Models;
using System.Data;
using System.ServiceModel;

namespace SOAPWebServicesCore.Services
{
    /// <summary>
    /// Service contract for data retrieval operations.
    /// </summary>
    [ServiceContract(Namespace = "http://tempuri.org/")]
    public interface IGetDataService
    {
        /// <summary>
        /// Returns a simple hello world message.
        /// </summary>
        /// <returns>A hello world string.</returns>
        [OperationContract]
        string HelloWorld();

        /// <summary>
        /// Gets data for the specified name.
        /// </summary>
        /// <param name="name">The name to get data for.</param>
        /// <returns>Data associated with the provided name.</returns>
        [OperationContract]
        string GetData(string name);

        /// <summary>
        /// Retrieves a dataset containing various data elements.
        /// </summary>
        /// <returns>A dataset with the requested information.</returns>
        [OperationContract]
        DataSet GetDataSet();

        /// <summary>
        /// Generates a report based on the provided input parameters.
        /// </summary>
        /// <param name="reportInput">The input parameters for the report generation, passed by reference.</param>
        /// <returns>A dataset containing the generated report data.</returns>
        [OperationContract]
        DataSet GetReport(ref ReportInput reportInput);
    }
}