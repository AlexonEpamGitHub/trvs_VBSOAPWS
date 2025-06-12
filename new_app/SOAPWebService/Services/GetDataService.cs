using System.Data;
using System.ServiceModel;
using SOAPWebService.Models;

namespace SOAPWebService.Services
{
    public class GetDataService : IGetDataService
    {
        private readonly ILogger<GetDataService> _logger;

        public GetDataService(ILogger<GetDataService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string HelloWorld()
        {
            _logger.LogInformation("HelloWorld method called at {Timestamp}", DateTime.UtcNow);
            return "Hello World";
        }

        public string GetData(string name)
        {
            _logger.LogInformation("GetData method called with name: {Name} at {Timestamp}", name ?? "null", DateTime.UtcNow);
            
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("GetData called with null or empty name parameter");
                return "Hello Guest, this is a simple SOAP web service response.";
            }
            
            return $"Hello {name}, this is a simple SOAP web service response.";
        }

        public DataSet GetDataSet()
        {
            _logger.LogInformation("GetDataSet method called at {Timestamp}", DateTime.UtcNow);
            
            try
            {
                var ds = new DataSet("SampleDataSet");
                var dt = new DataTable("SampleTable");
                
                // Define columns
                dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                
                // Adding sample data
                dt.Rows.Add(1, "Alice");
                dt.Rows.Add(2, "Bob");
                
                ds.Tables.Add(dt);
                
                _logger.LogInformation("DataSet created successfully with {TableCount} table(s) and {RowCount} row(s)", 
                    ds.Tables.Count, dt.Rows.Count);
                
                return ds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating DataSet");
                throw;
            }
        }

        public DataSet GetReport(ReportInput reportInput)
        {
            _logger.LogInformation("GetReport method called with ReportName: {ReportName} at {Timestamp}", 
                reportInput?.ReportName ?? "null", DateTime.UtcNow);
            
            if (reportInput == null)
            {
                _logger.LogWarning("GetReport called with null ReportInput parameter");
            }
            else
            {
                _logger.LogInformation("Processing report request for: {ReportName}", reportInput.ReportName);
            }
            
            try
            {
                var result = GetDataSet();
                _logger.LogInformation("Report data generated successfully");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating report for ReportName: {ReportName}", 
                    reportInput?.ReportName ?? "null");
                throw;
            }
        }
    }
}