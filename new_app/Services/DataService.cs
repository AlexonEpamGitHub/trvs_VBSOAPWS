using System.Data;
using SOAPWebServicesCore.Models;

namespace SOAPWebServicesCore.Services;

/// <summary>
/// Service class that provides data operations for SOAP web services.
/// Implements comprehensive logging and modern C# practices.
/// </summary>
public class DataService : IDataService
{
    private readonly ILogger<DataService> _logger;

    /// <summary>
    /// Initializes a new instance of the DataService class.
    /// </summary>
    /// <param name="logger">The logger instance for structured logging</param>
    /// <exception cref="ArgumentNullException">Thrown when logger is null</exception>
    public DataService(ILogger<DataService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Returns a simple "Hello World" greeting message.
    /// </summary>
    /// <returns>A string containing "Hello World"</returns>
    public string HelloWorld()
    {
        try
        {
            _logger.LogInformation("HelloWorld method called");
            const string response = "Hello World";
            _logger.LogDebug("HelloWorld method returning: {Response}", response);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in HelloWorld method");
            throw;
        }
    }

    /// <summary>
    /// Returns a personalized greeting message with the provided name.
    /// </summary>
    /// <param name="name">The name to include in the greeting</param>
    /// <returns>A personalized greeting string</returns>
    /// <exception cref="ArgumentException">Thrown when name is null or whitespace</exception>
    public string GetData(string name)
    {
        try
        {
            _logger.LogInformation("GetData method called with name: {Name}", name ?? "null");
            
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("GetData called with null or empty name parameter");
                throw new ArgumentException("Name parameter cannot be null or empty", nameof(name));
            }

            var response = $"Hello {name}, this is a simple SOAP web service response.";
            _logger.LogDebug("GetData method returning response for name: {Name}", name);
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
    /// The DataSet contains a table with ID and Name columns and sample records.
    /// </summary>
    /// <returns>A DataSet containing sample data with ID and Name columns</returns>
    public DataSet GetDataSet()
    {
        try
        {
            _logger.LogInformation("GetDataSet method called");
            
            var ds = new DataSet("SampleDataSet");
            var dt = new DataTable("SampleTable");
            
            // Define table structure
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            
            // Adding sample data
            dt.Rows.Add(1, "Alice");
            dt.Rows.Add(2, "Bob");
            dt.Rows.Add(3, "Charlie");
            dt.Rows.Add(4, "Diana");
            
            ds.Tables.Add(dt);
            
            _logger.LogDebug("GetDataSet method created DataSet with {TableCount} tables and {RowCount} rows", 
                ds.Tables.Count, dt.Rows.Count);
            
            return ds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetDataSet method");
            throw;
        }
    }

    /// <summary>
    /// Generates a report based on the provided ReportInput parameters.
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
            _logger.LogInformation("GetReport method called with ReportName: {ReportName}", 
                reportInput?.ReportName ?? "null");
            
            if (reportInput == null)
            {
                _logger.LogWarning("GetReport called with null reportInput parameter");
                throw new ArgumentNullException(nameof(reportInput), "ReportInput parameter cannot be null");
            }

            if (string.IsNullOrWhiteSpace(reportInput.ReportName))
            {
                _logger.LogWarning("GetReport called with null or empty ReportName");
                throw new ArgumentException("ReportName cannot be null or empty", nameof(reportInput));
            }

            // Create customized dataset based on report input
            var ds = new DataSet($"Report_{reportInput.ReportName}");
            var dt = new DataTable("ReportData");
            
            // Define report structure
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("ReportType", typeof(string));
            dt.Columns.Add("GeneratedDate", typeof(DateTime));
            
            // Generate report data based on input
            switch (reportInput.ReportName.ToUpperInvariant())
            {
                case "USERS":
                    dt.Rows.Add(1, "Alice Johnson", "Users", DateTime.Now);
                    dt.Rows.Add(2, "Bob Smith", "Users", DateTime.Now);
                    dt.Rows.Add(3, "Charlie Brown", "Users", DateTime.Now);
                    break;
                
                case "DEPARTMENTS":
                    dt.Rows.Add(1, "IT Department", "Departments", DateTime.Now);
                    dt.Rows.Add(2, "HR Department", "Departments", DateTime.Now);
                    dt.Rows.Add(3, "Finance Department", "Departments", DateTime.Now);
                    break;
                
                default:
                    // Default report with sample data
                    dt.Rows.Add(1, "Default Entry 1", reportInput.ReportName, DateTime.Now);
                    dt.Rows.Add(2, "Default Entry 2", reportInput.ReportName, DateTime.Now);
                    break;
            }
            
            ds.Tables.Add(dt);
            
            _logger.LogDebug("GetReport method generated report '{ReportName}' with {RowCount} rows", 
                reportInput.ReportName, dt.Rows.Count);
            
            return ds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetReport method with ReportName: {ReportName}", 
                reportInput?.ReportName ?? "null");
            throw;
        }
    }
}