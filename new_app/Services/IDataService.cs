using System.Data;
using System.ServiceModel;
using SOAPWebServicesCore.Models;

namespace SOAPWebServicesCore.Services;

/// <summary>
/// Service contract interface defining SOAP operations for data service functionality.
/// This interface provides a comprehensive set of data access methods including greeting operations,
/// dataset retrieval, and report generation capabilities for backward compatibility with legacy systems.
/// </summary>
/// <remarks>
/// This service contract maintains compatibility with legacy VB.NET WebMethods while following
/// modern SOA patterns and .NET 8 coding standards. All operations are exposed as SOAP endpoints
/// using WCF service model infrastructure.
/// </remarks>
[ServiceContract(Namespace = "http://tempuri.org/")]
public interface IDataService
{
    /// <summary>
    /// Simple greeting method that returns a hello world message.
    /// This method serves as a basic connectivity test and service health check endpoint.
    /// </summary>
    /// <returns>
    /// A string containing a hello world greeting message in the format "Hello World".
    /// </returns>
    /// <example>
    /// <code>
    /// var service = new DataService();
    /// string greeting = service.HelloWorld();
    /// // Returns: "Hello World"
    /// </code>
    /// </example>
    [OperationContract]
    string HelloWorld();

    /// <summary>
    /// Personalized greeting method that returns a customized message with the provided name.
    /// This method demonstrates parameter passing and string manipulation capabilities.
    /// </summary>
    /// <param name="name">
    /// The name to include in the personalized greeting message. 
    /// If null or empty, a default message will be returned.
    /// </param>
    /// <returns>
    /// A string containing a personalized greeting message with the specified name
    /// in the format "Hello [name]" or a default message if name is not provided.
    /// </returns>
    /// <example>
    /// <code>
    /// var service = new DataService();
    /// string personalizedGreeting = service.GetData("John");
    /// // Returns: "Hello John"
    /// 
    /// string defaultGreeting = service.GetData("");
    /// // Returns: "Hello World" (default behavior)
    /// </code>
    /// </example>
    [OperationContract]
    string GetData(string name);

    /// <summary>
    /// Returns a sample DataSet containing user information with ID and Name columns.
    /// The DataSet includes predefined records for demonstration and testing purposes.
    /// This method showcases structured data retrieval capabilities.
    /// </summary>
    /// <returns>
    /// A DataSet containing a single DataTable named "Users" with the following structure:
    /// - ID column (int): Unique identifier for each user
    /// - Name column (string): User's display name
    /// The DataSet includes sample records for Alice and Bob as demonstration data.
    /// </returns>
    /// <example>
    /// <code>
    /// var service = new DataService();
    /// DataSet userDataSet = service.GetDataSet();
    /// DataTable usersTable = userDataSet.Tables["Users"];
    /// 
    /// foreach (DataRow row in usersTable.Rows)
    /// {
    ///     int id = (int)row["ID"];
    ///     string name = (string)row["Name"];
    ///     Console.WriteLine($"User {id}: {name}");
    /// }
    /// // Output:
    /// // User 1: Alice
    /// // User 2: Bob
    /// </code>
    /// </example>
    [OperationContract]
    DataSet GetDataSet();

    /// <summary>
    /// Generates a report based on the provided input parameters and returns the results as a DataSet.
    /// This method processes the ReportInput parameter to create customized report data with
    /// flexible filtering, sorting, and formatting options.
    /// </summary>
    /// <param name="reportInput">
    /// The report input parameters containing criteria and configuration for report generation.
    /// This object should include properties such as report type, date range, filters,
    /// and formatting options. Cannot be null.
    /// </param>
    /// <returns>
    /// A DataSet containing the generated report data based on the input parameters.
    /// The structure and content of the DataSet varies depending on the report type
    /// and parameters specified in the reportInput object. May contain multiple
    /// DataTables for complex reports with related data.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when reportInput parameter is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when reportInput contains invalid or incompatible parameters.
    /// </exception>
    /// <example>
    /// <code>
    /// var service = new DataService();
    /// var reportInput = new ReportInput
    /// {
    ///     ReportType = "Sales",
    ///     StartDate = DateTime.Now.AddDays(-30),
    ///     EndDate = DateTime.Now,
    ///     IncludeDetails = true
    /// };
    /// 
    /// DataSet reportData = service.GetReport(reportInput);
    /// DataTable summaryTable = reportData.Tables["Summary"];
    /// DataTable detailsTable = reportData.Tables["Details"];
    /// 
    /// // Process report data as needed
    /// Console.WriteLine($"Report contains {reportData.Tables.Count} tables");
    /// </code>
    /// </example>
    [OperationContract]
    DataSet GetReport(ReportInput reportInput);
}