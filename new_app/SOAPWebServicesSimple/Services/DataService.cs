using SOAPWebServicesSimple.Models;
using System;
using System.Data;
using System.ServiceModel;

namespace SOAPWebServicesSimple.Services;

/// <summary>
/// Implements the IDataService interface to provide various data operations
/// </summary>
public class DataService : IDataService
{
    /// <summary>
    /// Returns a simple Hello World greeting
    /// </summary>
    /// <returns>A greeting string</returns>
    public string HelloWorld()
    {
        return "Hello World";
    }

    /// <summary>
    /// Returns a personalized greeting for the specified name
    /// </summary>
    /// <param name="name">The name to include in the greeting</param>
    /// <returns>A personalized greeting string</returns>
    public string GetData(string name)
    {
        return $"Hello {name}, this is a simple SOAP web service response.";
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
        return dataSet;
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
        Console.WriteLine($"Processing report at {DateTime.UtcNow}. Report ID: {reportInput.ReportId}, Type: {reportInput.ReportType}");
    
        // In a real implementation, we would use reportInput to filter or customize the data
        // For now, we're just returning the sample dataset
        return GetDataSet();
    }
}
