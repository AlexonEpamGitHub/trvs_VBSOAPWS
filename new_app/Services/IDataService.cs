using System.Data;
using System.ServiceModel;
using SOAPWebServicesCore.Models;

namespace SOAPWebServicesCore.Services;

/// <summary>
/// Service contract interface defining SOAP operations for data service functionality.
/// This interface mirrors the legacy VB.NET GetDataService.asmx.vb WebMethods while following modern SOA patterns.
/// </summary>
[ServiceContract(Namespace = "http://tempuri.org/")]
public interface IDataService
{
    /// <summary>
    /// Simple greeting method that returns a hello world message.
    /// </summary>
    /// <returns>A string containing a hello world greeting message.</returns>
    [OperationContract]
    string HelloWorld();

    /// <summary>
    /// Personalized greeting method that returns a customized message with the provided name.
    /// </summary>
    /// <param name="name">The name to include in the personalized greeting message.</param>
    /// <returns>A string containing a personalized greeting message with the specified name.</returns>
    [OperationContract]
    string GetData(string name);

    /// <summary>
    /// Returns a sample DataSet containing user information with ID and Name columns.
    /// The DataSet includes predefined records for Alice and Bob as demonstration data.
    /// </summary>
    /// <returns>A DataSet containing sample user records with ID and Name columns.</returns>
    [OperationContract]
    DataSet GetDataSet();

    /// <summary>
    /// Generates a report based on the provided input parameters and returns the results as a DataSet.
    /// This method processes the ReportInput parameter to create customized report data.
    /// </summary>
    /// <param name="reportInput">The report input parameters containing criteria and configuration for report generation.</param>
    /// <returns>A DataSet containing the generated report data based on the input parameters.</returns>
    [OperationContract]
    DataSet GetReport(ReportInput reportInput);
}