using SOAPWebServicesCore.Models;
using System.Data;
using System.ServiceModel;

namespace SOAPWebServicesCore.Services
{
    public class GetDataService : IGetDataService
    {
        /// <summary>
        /// Returns a simple Hello World message
        /// </summary>
        /// <returns>A greeting message</returns>
        public string HelloWorld()
        {
            return "Hello World";
        }

        /// <summary>
        /// Returns a personalized greeting message
        /// </summary>
        /// <param name="name">Name of the person to greet</param>
        /// <returns>A personalized greeting message</returns>
        public string GetData(string name)
        {
            return $"Hello {name}, this is a simple SOAP web service response.";
        }

        /// <summary>
        /// Creates and returns a sample DataSet
        /// </summary>
        /// <returns>A DataSet containing sample data</returns>
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

        /// <summary>
        /// Returns a report based on the input parameters
        /// </summary>
        /// <param name="reportInput">Input parameters for the report</param>
        /// <returns>A DataSet containing the report data</returns>
        public DataSet GetReport(ref ReportInput reportInput)
        {
            return GetDataSet();
        }
    }
}