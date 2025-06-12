using System.Data;
using SOAPWebServicesCore.Models;

namespace SOAPWebServicesCore.Services;

/// <summary>
/// Service class that provides data operations for SOAP web services.
/// Implements comprehensive logging and modern C# practices following .NET 8 standards.
/// </summary>
public class DataService : IDataService
{
    private readonly ILogger<DataService> _logger;

    /// <summary>
    /// Initializes a new instance of the DataService class with dependency injection.
    /// </summary>
    /// <param name="logger">The logger instance for structured logging</param>
    /// <exception cref="ArgumentNullException">Thrown when logger is null</exception>
    public DataService(ILogger<DataService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("DataService instance created");
    }

    /// <summary>
    /// Returns a simple "Hello World" greeting message.
    /// </summary>
    /// <returns>A string containing "Hello World"</returns>
    public string HelloWorld()
    {
        try
        {
            _logger.LogInformation("HelloWorld method invoked");
            const string response = "Hello World";
            _logger.LogDebug("HelloWorld method completed successfully with response: {Response}", response);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred in HelloWorld method");
            throw;
        }
    }

    /// <summary>
    /// Returns a personalized greeting message with the provided name using string interpolation.
    /// </summary>
    /// <param name="name">The name to include in the greeting</param>
    /// <returns>A personalized greeting string</returns>
    /// <exception cref="ArgumentException">Thrown when name is null or whitespace</exception>
    public string GetData(string name)
    {
        try
        {
            _logger.LogInformation("GetData method invoked with name: {Name}", name ?? "null");
            
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("GetData method called with invalid name parameter");
                throw new ArgumentException("Name parameter cannot be null or empty", nameof(name));
            }

            var response = $"Hello {name}, this is a simple SOAP web service response.";
            _logger.LogDebug("GetData method completed successfully for name: {Name}", name);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetData method with name: {Name}", name ?? "null");
            throw;
        }
    }

    /// <summary>
    /// Creates and returns a sample DataSet with predefined structure and data.
    /// The DataSet contains a table with ID and Name columns including Alice and Bob records.
    /// </summary>
    /// <returns>A DataSet containing sample data with ID and Name columns</returns>
    public DataSet GetDataSet()
    {
        try
        {
            _logger.LogInformation("GetDataSet method invoked");
            
            var dataSet = new DataSet("SampleDataSet");
            var dataTable = new DataTable("SampleTable");
            
            // Define table structure with proper column types
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));
            
            // Add sample data as specified - Alice and Bob
            dataTable.Rows.Add(1, "Alice");
            dataTable.Rows.Add(2, "Bob");
            
            dataSet.Tables.Add(dataTable);
            
            _logger.LogDebug("GetDataSet method created DataSet with {TableCount} table(s) and {RowCount} row(s)", 
                dataSet.Tables.Count, dataTable.Rows.Count);
            
            return dataSet;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetDataSet method");
            throw;
        }
    }

    /// <summary>
    /// Generates a report based on the provided ReportInput parameters with null-safe handling.
    /// Returns a DataSet containing the requested report data.
    /// </summary>
    /// <param name="reportInput">The input parameters for report generation</param>
    /// <returns>A DataSet containing the generated report data</returns>
    /// <exception cref="ArgumentNullException">Thrown when reportInput is null</exception>
    /// <exception cref="ArgumentException">Thrown when ReportName is null or empty</exception>
    public DataSet GetReport(ReportInput reportInput)
    {
        try
        {
            _logger.LogInformation("GetReport method invoked with ReportName: {ReportName}", 
                reportInput?.ReportName ?? "null");
            
            // Null-safe parameter validation
            if (reportInput is null)
            {
                _logger.LogWarning("GetReport method called with null reportInput parameter");
                throw new ArgumentNullException(nameof(reportInput), "ReportInput parameter cannot be null");
            }

            if (string.IsNullOrWhiteSpace(reportInput.ReportName))
            {
                _logger.LogWarning("GetReport method called with null or empty ReportName");
                throw new ArgumentException("ReportName cannot be null or empty", nameof(reportInput));
            }

            // Create report dataset with dynamic naming
            var reportDataSet = new DataSet($"Report_{reportInput.ReportName}");
            var reportTable = new DataTable("ReportData");
            
            // Define comprehensive report structure
            reportTable.Columns.Add("ID", typeof(int));
            reportTable.Columns.Add("Name", typeof(string));
            reportTable.Columns.Add("ReportType", typeof(string));
            reportTable.Columns.Add("GeneratedDate", typeof(DateTime));
            
            var currentDateTime = DateTime.Now;
            
            // Generate report data based on report type with modern switch expression
            var reportData = reportInput.ReportName.ToUpperInvariant() switch
            {
                "USERS" => new[]
                {
                    new object[] { 1, "Alice Johnson", "Users", currentDateTime },
                    new object[] { 2, "Bob Smith", "Users", currentDateTime },
                    new object[] { 3, "Charlie Brown", "Users", currentDateTime }
                },
                "DEPARTMENTS" => new[]
                {
                    new object[] { 1, "IT Department", "Departments", currentDateTime },
                    new object[] { 2, "HR Department", "Departments", currentDateTime },
                    new object[] { 3, "Finance Department", "Departments", currentDateTime }
                },
                _ => new[]
                {
                    new object[] { 1, "Default Entry 1", reportInput.ReportName, currentDateTime },
                    new object[] { 2, "Default Entry 2", reportInput.ReportName, currentDateTime }
                }
            };
            
            // Add rows to the report table
            foreach (var row in reportData)
            {
                reportTable.Rows.Add(row);
            }
            
            reportDataSet.Tables.Add(reportTable);
            
            _logger.LogDebug("GetReport method generated report '{ReportName}' with {RowCount} row(s)", 
                reportInput.ReportName, reportTable.Rows.Count);
            
            return reportDataSet;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetReport method with ReportName: {ReportName}", 
                reportInput?.ReportName ?? "null");
            throw;
        }
    }
}
