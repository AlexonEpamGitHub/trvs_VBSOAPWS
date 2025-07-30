using SOAPWebServicesSimple.Models;
using System.Data;
using System.ServiceModel;
using System.Threading.Tasks;

namespace SOAPWebServicesSimple.Services;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface IDataService
{
    /// <summary>
    /// Returns a hello world message
    /// </summary>
    /// <returns>A greeting message</returns>
    [OperationContract]
    string HelloWorld();

    /// <summary>
    /// Asynchronously returns a hello world message
    /// </summary>
    /// <returns>A task containing a greeting message</returns>
    [OperationContract]
    Task<string> HelloWorldAsync();

    /// <summary>
    /// Returns a personalized greeting message
    /// </summary>
    /// <param name="userName">The name of the user to greet</param>
    /// <returns>A personalized greeting message</returns>
    [OperationContract]
    string GetData(string userName);

    /// <summary>
    /// Asynchronously returns a personalized greeting message
    /// </summary>
    /// <param name="userName">The name of the user to greet</param>
    /// <returns>A task containing a personalized greeting message</returns>
    [OperationContract]
    Task<string> GetDataAsync(string userName);

    /// <summary>
    /// Retrieves a sample dataset
    /// </summary>
    /// <returns>A dataset containing sample data</returns>
    [OperationContract]
    DataSet GetDataSet();

    /// <summary>
    /// Asynchronously retrieves a sample dataset
    /// </summary>
    /// <returns>A task containing a dataset with sample data</returns>
    [OperationContract]
    Task<DataSet> GetDataSetAsync();

    /// <summary>
    /// Generates a report based on the provided input parameters
    /// </summary>
    /// <param name="reportParameters">Input parameters for the report generation</param>
    /// <returns>A dataset containing the report data</returns>
    [OperationContract]
    DataSet GetReport(ref ReportInput reportParameters);

    /// <summary>
    /// Asynchronously generates a report based on the provided input parameters
    /// </summary>
    /// <param name="reportParameters">Input parameters for the report generation</param>
    /// <returns>A task containing a dataset with the report data</returns>
    [OperationContract]
    Task<DataSet> GetReportAsync(ReportInput reportParameters);
}
