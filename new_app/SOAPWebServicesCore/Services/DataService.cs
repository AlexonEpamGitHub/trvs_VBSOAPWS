using SOAPWebServicesCore.Models;
using System;
using System.Data;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace SOAPWebServicesCore.Services
{
    [ServiceContract(Namespace = "http://tempuri.org/")]
    public interface IDataService
    {
        [OperationContract]
        string HelloWorld();

        [OperationContract]
        string GetData(string name);

        [OperationContract]
        DataSet GetDataSet();

        [OperationContract]
        DataSet GetReport(ReportInput reportInput);
    }

    [ServiceBehavior(Namespace = "http://tempuri.org/")]
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
            // Check input parameter
            if (reportInput == null)
            {
                throw new ArgumentNullException(nameof(reportInput));
            }
            
            // Use the report name if provided
            var ds = GetDataSet();
            if (!string.IsNullOrEmpty(reportInput.ReportName))
            {
                ds.DataSetName = reportInput.ReportName;
            }
            
            return ds;
        }
    }
}