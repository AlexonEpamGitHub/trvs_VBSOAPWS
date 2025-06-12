using System.Data;
using System.ServiceModel;
using SOAPWebServicesCore.Models;

namespace SOAPWebServicesCore.Services;

/// <summary>
/// Data service implementation that provides SOAP web service functionality
/// for retrieving data and generating reports. This service implements the
/// IDataService interface and maintains compatibility with legacy VB.NET service.
/// </summary>
[ServiceBehavior(Namespace = "http://tempuri.org/")]
public class DataService : IDataService
{
    private readonly ILogger<DataService> _logger;

    /// <summary>
    /// Initializes a new instance of the DataService class.
    /// </summary>
    /// <param name="logger">The logger instance for logging service operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when logger is null.</exception>
    public DataService(ILogger<DataService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Returns a simple "Hello World" greeting message.
    /// This method provides a basic connectivity test for the SOAP service.
    /// </summary>
    /// <returns>A string containing "Hello World".</returns>
    [OperationBehavior]
    public string HelloWorld()
    {
        _logger.LogInformation("HelloWorld method called at {Timestamp}", DateTime.UtcNow);
        
        try
        {
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
    /// Returns a personalized greeting message for the specified name.
    /// </summary>
    /// <param name="name">The name to include in the greeting. Can be null or empty.</param>
    /// <returns>A personalized greeting message as a string.</returns>
    [OperationBehavior]
    public string GetData(string name)
    {
        _logger.LogInformation("GetData method called with name: '{Name}' at {Timestamp}", 
            name ?? "null", DateTime.UtcNow);
        
        try
        {
            // Handle null or empty name with modern C# null-conditional operators
            var safeName = string.IsNullOrWhiteSpace(name) ? "Guest" : name.Trim();
            
            // Use string interpolation for modern C# syntax
            var response = $"Hello {safeName}, this is a simple SOAP web service response.";
            
            _logger.LogDebug("GetData method returning response for name '{SafeName}': {Response}", 
                safeName, response);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetData method for name: '{Name}'", name);
            throw;
        }
    }

    /// <summary>
    /// Returns a sample DataSet containing structured data.
    /// This method demonstrates returning complex data types through SOAP.
    /// </summary>
    /// <returns>A DataSet containing sample table data with ID and Name columns.</returns>
    [OperationBehavior]
    public DataSet GetDataSet()
    {
        _logger.LogInformation("GetDataSet method called at {Timestamp}", DateTime.UtcNow);
        
        try
        {
            // Create DataSet with proper naming for SOAP serialization
            var ds = new DataSet("SampleDataSet");
            var dt = new DataTable("SampleTable");
            
            // Define table structure
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("CreatedDate", typeof(DateTime));
            dt.Columns.Add("IsActive", typeof(bool));
            
            // Adding comprehensive sample data to match legacy functionality
            dt.Rows.Add(1, "Alice Johnson", DateTime.Now.AddDays(-30), true);
            dt.Rows.Add(2, "Bob Smith", DateTime.Now.AddDays(-15), true);
            dt.Rows.Add(3, "Charlie Brown", DateTime.Now.AddDays(-7), false);
            dt.Rows.Add(4, "Diana Prince", DateTime.Now.AddDays(-2), true);
            
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
    /// Generates a report based on the provided report input parameters.
    /// This method processes report requests and returns structured data as a DataSet.
    /// </summary>
    /// <param name="reportInput">The report input parameters containing report name and other configuration options. Can be null.</param>
    /// <returns>A DataSet containing the requested report data.</returns>
    /// <exception cref="ArgumentException">Thrown when report input contains invalid parameters.</exception>
    [OperationBehavior]
    public DataSet GetReport(ReportInput reportInput)
    {
        var reportName = reportInput?.ReportName ?? "DefaultReport";
        _logger.LogInformation("GetReport method called with ReportName: '{ReportName}', StartDate: {StartDate}, EndDate: {EndDate} at {Timestamp}", 
            reportName, 
            reportInput?.StartDate?.ToString("yyyy-MM-dd") ?? "null",
            reportInput?.EndDate?.ToString("yyyy-MM-dd") ?? "null",
            DateTime.UtcNow);
        
        try
        {
            // Comprehensive null checking and validation
            if (reportInput != null)
            {
                // Validate date range if provided
                if (reportInput.StartDate.HasValue && reportInput.EndDate.HasValue && 
                    reportInput.StartDate > reportInput.EndDate)
                {
                    var errorMessage = $"Invalid date range: StartDate ({reportInput.StartDate:yyyy-MM-dd}) cannot be greater than EndDate ({reportInput.EndDate:yyyy-MM-dd})";
                    _logger.LogWarning(errorMessage);
                    throw new ArgumentException(errorMessage, nameof(reportInput));
                }
            }
            
            // Create report-specific DataSet
            var reportDs = new DataSet($"ReportDataSet_{reportName}");
            DataTable reportTable;
            
            // Generate different report data based on report name using modern C# switch expression
            reportTable = reportName?.ToLowerInvariant() switch
            {
                "sales" or "salesreport" => GenerateSalesReportData(reportInput),
                "users" or "userreport" => GenerateUserReportData(reportInput),
                "activity" or "activityreport" => GenerateActivityReportData(reportInput),
                _ => GenerateDefaultReportData(reportInput)
            };
            
            reportTable.TableName = $"{reportName}Table";
            reportDs.Tables.Add(reportTable);
            
            _logger.LogDebug("GetReport method generated report '{ReportName}' with {RowCount} rows", 
                reportName, reportTable.Rows.Count);
            
            return reportDs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetReport method for ReportName: '{ReportName}'", reportName);
            throw;
        }
    }

    /// <summary>
    /// Generates sales report data based on the provided report input parameters.
    /// </summary>
    /// <param name="reportInput">The report input parameters for filtering sales data.</param>
    /// <returns>A DataTable containing sales report data.</returns>
    private DataTable GenerateSalesReportData(ReportInput reportInput)
    {
        var dt = new DataTable("SalesData");
        
        dt.Columns.Add("SaleID", typeof(int));
        dt.Columns.Add("CustomerName", typeof(string));
        dt.Columns.Add("Amount", typeof(decimal));
        dt.Columns.Add("SaleDate", typeof(DateTime));
        dt.Columns.Add("Region", typeof(string));
        
        // Generate sample sales data with date filtering
        var baseDate = reportInput?.StartDate ?? DateTime.Now.AddDays(-30);
        var endDate = reportInput?.EndDate ?? DateTime.Now;
        
        var salesData = new[]
        {
            new { ID = 1, Customer = "ABC Corp", Amount = 1500.00m, Days = -25, Region = "North" },
            new { ID = 2, Customer = "XYZ Ltd", Amount = 2750.50m, Days = -20, Region = "South" },
            new { ID = 3, Customer = "Tech Solutions", Amount = 950.25m, Days = -15, Region = "East" },
            new { ID = 4, Customer = "Global Industries", Amount = 3200.75m, Days = -10, Region = "West" },
            new { ID = 5, Customer = "Innovation Hub", Amount = 1825.00m, Days = -5, Region = "North" }
        };
        
        foreach (var sale in salesData)
        {
            var saleDate = baseDate.AddDays(sale.Days);
            if (saleDate >= baseDate && saleDate <= endDate)
            {
                dt.Rows.Add(sale.ID, sale.Customer, sale.Amount, saleDate, sale.Region);
            }
        }
        
        return dt;
    }

    /// <summary>
    /// Generates user report data based on the provided report input parameters.
    /// </summary>
    /// <param name="reportInput">The report input parameters for filtering user data.</param>
    /// <returns>A DataTable containing user report data.</returns>
    private DataTable GenerateUserReportData(ReportInput reportInput)
    {
        var dt = new DataTable("UserData");
        
        dt.Columns.Add("UserID", typeof(int));
        dt.Columns.Add("UserName", typeof(string));
        dt.Columns.Add("Email", typeof(string));
        dt.Columns.Add("LastLogin", typeof(DateTime));
        dt.Columns.Add("IsActive", typeof(bool));
        dt.Columns.Add("Role", typeof(string));
        
        // Generate sample user data
        var userData = new[]
        {
            new { ID = 1, Name = "John Doe", Email = "john.doe@example.com", Days = -2, Active = true, Role = "Admin" },
            new { ID = 2, Name = "Jane Smith", Email = "jane.smith@example.com", Days = -1, Active = true, Role = "User" },
            new { ID = 3, Name = "Mike Johnson", Email = "mike.johnson@example.com", Days = -5, Active = false, Role = "User" },
            new { ID = 4, Name = "Sarah Wilson", Email = "sarah.wilson@example.com", Days = -3, Active = true, Role = "Manager" },
            new { ID = 5, Name = "David Brown", Email = "david.brown@example.com", Days = -7, Active = true, Role = "User" }
        };
        
        foreach (var user in userData)
        {
            var lastLogin = DateTime.Now.AddDays(user.Days);
            dt.Rows.Add(user.ID, user.Name, user.Email, lastLogin, user.Active, user.Role);
        }
        
        return dt;
    }

    /// <summary>
    /// Generates activity report data based on the provided report input parameters.
    /// </summary>
    /// <param name="reportInput">The report input parameters for filtering activity data.</param>
    /// <returns>A DataTable containing activity report data.</returns>
    private DataTable GenerateActivityReportData(ReportInput reportInput)
    {
        var dt = new DataTable("ActivityData");
        
        dt.Columns.Add("ActivityID", typeof(int));
        dt.Columns.Add("ActivityType", typeof(string));
        dt.Columns.Add("UserName", typeof(string));
        dt.Columns.Add("Timestamp", typeof(DateTime));
        dt.Columns.Add("Details", typeof(string));
        dt.Columns.Add("Success", typeof(bool));
        
        // Generate sample activity data
        var activities = new[]
        {
            new { ID = 1, Type = "Login", User = "john.doe", Hours = -2, Details = "Successful login from IP 192.168.1.100", Success = true },
            new { ID = 2, Type = "Data Export", User = "jane.smith", Hours = -4, Details = "Exported sales report", Success = true },
            new { ID = 3, Type = "Login", User = "mike.johnson", Hours = -6, Details = "Failed login attempt", Success = false },
            new { ID = 4, Type = "Report Generation", User = "sarah.wilson", Hours = -8, Details = "Generated user activity report", Success = true },
            new { ID = 5, Type = "Data Import", User = "david.brown", Hours = -12, Details = "Imported customer data", Success = true }
        };
        
        foreach (var activity in activities)
        {
            var timestamp = DateTime.Now.AddHours(activity.Hours);
            dt.Rows.Add(activity.ID, activity.Type, activity.User, timestamp, activity.Details, activity.Success);
        }
        
        return dt;
    }

    /// <summary>
    /// Generates default report data when no specific report type is requested.
    /// </summary>
    /// <param name="reportInput">The report input parameters.</param>
    /// <returns>A DataTable containing default report data.</returns>
    private DataTable GenerateDefaultReportData(ReportInput reportInput)
    {
        _logger.LogDebug("Generating default report data as fallback");
        
        // Return enhanced version of the basic dataset for default reports
        var dt = new DataTable("DefaultReportData");
        
        dt.Columns.Add("ID", typeof(int));
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("CreatedDate", typeof(DateTime));
        dt.Columns.Add("IsActive", typeof(bool));
        dt.Columns.Add("Category", typeof(string));
        dt.Columns.Add("Value", typeof(decimal));
        
        // Enhanced default data
        dt.Rows.Add(1, "Alice Johnson", DateTime.Now.AddDays(-30), true, "Category A", 100.50m);
        dt.Rows.Add(2, "Bob Smith", DateTime.Now.AddDays(-15), true, "Category B", 250.75m);
        dt.Rows.Add(3, "Charlie Brown", DateTime.Now.AddDays(-7), false, "Category A", 75.25m);
        dt.Rows.Add(4, "Diana Prince", DateTime.Now.AddDays(-2), true, "Category C", 400.00m);
        dt.Rows.Add(5, "Edward Wilson", DateTime.Now.AddDays(-1), true, "Category B", 325.80m);
        
        return dt;
    }
}