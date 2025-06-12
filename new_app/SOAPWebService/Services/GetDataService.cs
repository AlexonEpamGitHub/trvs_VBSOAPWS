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
            _logger = logger;
        }

        public string HelloWorld()
        {
            _logger.LogInformation("HelloWorld method called");
            return "Hello World";
        }

        public string GetData(string name)
        {
            _logger.LogInformation("GetData method called with name: {Name}", name);
            return $"Hello {name}, this is a simple SOAP web service response.";
        }

        public DataSet GetDataSet()
        {
            _logger.LogInformation("GetDataSet method called");
            
            var ds = new DataSet("SampleDataSet");
            var dt = new DataTable("SampleTable");
            
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            
            // Adding sample data
            dt.Rows.Add(1, "Alice");
            dt.Rows.Add(2, "Bob");
            
            ds.Tables.Add(dt);
            return ds;
        }

        public DataSet GetReport(ReportInput reportInput)
        {
            _logger.LogInformation("GetReport method called with ReportName: {ReportName}", 
                reportInput?.ReportName ?? "null");
            
            return GetDataSet();
        }
    }
}