using SOAPWebServicesCore.Models;
using System.Data;
using System.ServiceModel;

namespace SOAPWebServicesCore.Services
{
    public class GetDataService : IGetDataService
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

        public DataSet GetReport(ref ReportInput reportInput)
        {
            return GetDataSet();
        }
    }
}
