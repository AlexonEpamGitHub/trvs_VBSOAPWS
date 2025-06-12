using System.Data;
using System.ServiceModel;
using SOAPWebServicesCore.Models;

namespace SOAPWebServicesCore.Services;

/// <summary>
/// Data service contract for SOAP web service operations
/// </summary>
[ServiceContract(Namespace = "http://tempuri.org/")]
public interface IDataService
{
    /// <summary>
    /// Returns a simple hello world message
    /// </summary>
    /// <returns>Hello world string</returns>
    [OperationContract]
    string HelloWorld();

    /// <summary>
    /// Gets data based on the provided name parameter
    /// </summary>
    /// <param name="name">The name parameter for data retrieval</param>
    /// <returns>Data string based on the provided name</returns>
    [OperationContract]
    string GetData(string name);

    /// <summary>
    /// Retrieves a DataSet containing sample data
    /// </summary>
    /// <returns>DataSet with sample data</returns>
    [OperationContract]
    DataSet GetDataSet();

    /// <summary>
    /// Generates a report based on the provided input parameters
    /// </summary>
    /// <param name="reportInput">The report input parameters</param>
    /// <returns>DataSet containing the report data</returns>
    [OperationContract]
    DataSet GetReport(ReportInput reportInput);
}