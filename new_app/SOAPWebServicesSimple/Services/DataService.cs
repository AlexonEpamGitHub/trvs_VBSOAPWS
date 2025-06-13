using System.Data;
using SOAPWebServicesSimple.Models;

namespace SOAPWebServicesSimple.Services
{
    /// <summary>
    /// Implementation of the SOAP web service that replaces the legacy GetDataService.asmx.vb
    /// </summary>
    public class DataService : IDataService
    {
        /// <summary>
        /// Returns a "Hello World" string
        /// </summary>
        /// <returns>A simple greeting message</returns>
        public string HelloWorld()
        {
            return "Hello World";
        }

        /// <summary>
        /// Returns a personalized greeting message with the provided name
        /// </summary>
        /// <param name="name">Name to use in the greeting</param>
        /// <returns>A personalized greeting message</returns>
        public string GetData(string name)
        {
            return $"Hello {name}, this is a simple SOAP web service response.";
        }

        /// <summary>
        /// Creates and returns a sample data set with a table and sample data
        /// </summary>
        /// <returns>A DataSet containing a table with sample data</returns>
        public DataSet GetDataSet()
        {
            DataSet ds = new("SampleDataSet");
            DataTable dt = new("SampleTable");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            
            // Adding some sample data
            dt.Rows.Add(1, "Alice");
            dt.Rows.Add(2, "Bob");
            ds.Tables.Add(dt);
            return ds;
        }

        /// <summary>
        /// Returns a dataset based on the provided report input
        /// </summary>
        /// <param name="reportInput">Input containing the report name</param>
        /// <returns>A DataSet containing the requested report data</returns>
        public DataSet GetReport(ReportInput reportInput)
        {
            // Note: In the original VB code this was a ByRef parameter
            // In CoreWCF, parameters are implicitly handled as references for complex types
            
            // In a real implementation, we would use the reportInput parameter
            // to determine which specific report data to return
            return GetDataSet();
        }
    }
}