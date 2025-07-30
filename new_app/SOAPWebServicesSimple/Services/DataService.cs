using SOAPWebServicesSimple.Models;
using System;
using System.Data;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SOAPWebServicesSimple.Services;

/// <summary>
/// Implements the IDataService interface to provide various data operations
/// </summary>
public class DataService : IDataService
{
    private readonly ILogger<DataService> _logger;
    
    /// <summary>
    /// Initializes a new instance of the DataService class
    /// </summary>
    /// <param name="logger">The logger instance for this service</param>
    public DataService(ILogger<DataService> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Returns a simple Hello World greeting
    /// </summary>
    /// <returns>A greeting string</returns>
    public string HelloWorld()
    {
        _logger.LogInformation("HelloWorld method called");
        return "Hello World";
    }
    
    /// <summary>
    /// Returns a simple Hello World greeting asynchronously
    /// </summary>
    /// <returns>A Task containing greeting string</returns>
    public async Task<string> HelloWorldAsync()
    {
        _logger.LogInformation("{MethodName} method called", nameof(HelloWorldAsync));
        return await Task.FromResult("Hello World");
    }

    /// <summary>
    /// Returns a personalized greeting for the specified name
    /// </summary>
    /// <param name="name">The name to include in the greeting</param>
    /// <returns>A personalized greeting string</returns>
    public string GetData(string name)
    {
        _logger.LogInformation("{MethodName} called with name: {Name}", nameof(GetData), name);
        return $"Hello {name}, this is a simple SOAP web service response.";
    }
    
    /// <summary>
    /// Returns a personalized greeting for the specified name asynchronously
    /// </summary>
    /// <param name="name">The name to include in the greeting</param>
    /// <returns>A Task containing personalized greeting string</returns>
    public async Task<string> GetDataAsync(string name)
    {
        _logger.LogInformation("{MethodName} called with name: {Name}", nameof(GetDataAsync), name);
        return await Task.FromResult($"Hello {name}, this is a simple asynchronous SOAP web service response.");
    }

    /// <summary>
    /// Creates and returns a sample DataSet containing demo data
    /// </summary>
    /// <returns>A DataSet with sample data</returns>
    public DataSet GetDataSet()
    {
        var dataSet = new DataSet("SampleDataSet");
        var dataTable = new DataTable("SampleTable");
        
        // Define table columns
        dataTable.Columns.Add("ID", typeof(int));
        dataTable.Columns.Add("Name", typeof(string));
        
        // Add sample data rows
        dataTable.Rows.Add(1, "Alice");
        dataTable.Rows.Add(2, "Bob");
        dataTable.Rows.Add(3, "Charlie");
        
        dataSet.Tables.Add(dataTable);
        _logger.LogInformation("{MethodName} returned dataset with {RowCount} rows", 
            nameof(GetDataSet), dataTable.Rows.Count);
        return dataSet;
    }
    
    /// <summary>
    /// Creates and returns a sample DataSet containing demo data asynchronously
    /// </summary>
    /// <returns>A Task containing a DataSet with sample data</returns>
    public async Task<DataSet> GetDataSetAsync()
    {
        _logger.LogInformation("{MethodName} called", nameof(GetDataSetAsync));
        return await Task.FromResult(GetDataSet());
    }

    /// <summary>
    /// Processes the report input and returns a data set
    /// </summary>
    /// <param name="reportInput">The report parameters that will be updated with processing information</param>
    /// <returns>A DataSet containing the report data based on input parameters</returns>
    /// <remarks>
    /// This method updates the ProcessedTimestamp property of the reportInput parameter
    /// to record when the report was generated
    /// </remarks>
    public DataSet GetReport(ref ReportInput reportInput)
    {
        // Update the ReportInput object to indicate it was processed
        reportInput.ProcessedTimestamp = DateTime.UtcNow;
        
        // Log the report processing
        _logger.LogInformation(
            "Processing report at {Timestamp}. Report ID: {ReportId}, Type: {ReportType}", 
            DateTime.UtcNow, 
            reportInput.ReportId, 
            reportInput.ReportType);
    
        // In a real implementation, we would use reportInput to filter or customize the data
        // For now, we're just returning the sample dataset
        return GetDataSet();
    }
    
    /// <summary>
    /// Processes the report input and returns a data set asynchronously
    /// </summary>
    /// <param name="reportInput">The report parameters that will be updated with processing information</param>
    /// <returns>A Task containing a DataSet with the report data based on input parameters</returns>
    /// <remarks>
    /// This method updates the ProcessedTimestamp property of the reportInput parameter
    /// to record when the report was generated
    /// </remarks>
    public async Task<DataSet> GetReportAsync(ReportInput reportInput)
    {
        // Update the ReportInput object to indicate it was processed
        reportInput.ProcessedTimestamp = DateTime.UtcNow;
        
        // Log the report processing
        _logger.LogInformation(
            "{MethodName}: Processing report at {Timestamp}. Report ID: {ReportId}, Type: {ReportType}", 
            nameof(GetReportAsync),
            DateTime.UtcNow, 
            reportInput.ReportId, 
            reportInput.ReportType);
    
        // In a real implementation, we would use reportInput to filter or customize the data
        // For now, we're just returning the sample dataset asynchronously
        return await GetDataSetAsync();
    }
}
