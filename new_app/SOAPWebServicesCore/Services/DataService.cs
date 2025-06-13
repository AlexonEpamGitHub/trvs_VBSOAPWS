using SOAPWebServicesCore.Models;
using System.Data;

namespace SOAPWebServicesCore.Services
{
    public class DataService : IDataService
    {
        public string HelloWorld()
        {
            return "Hello World";
        }
        
        public string GetData(string name)
        {
            return $"Hello {name}, this is a simple SOAP web service response.";
        }
        
        public DataSet GetDataSet()
        {
            DataSet ds = new DataSet("SampleDataSet");
            DataTable dt = new DataTable("SampleTable");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            
            // Adding some sample data
            dt.Rows.Add(1, "Alice");
            dt.Rows.Add(2, "Bob");
            ds.Tables.Add(dt);
            
            return ds;
        }
        
        public DataSet GetReport(ReportInput reportInput)
        {
            // Same implementation as the original service
            return GetDataSet();
        }
    }
}