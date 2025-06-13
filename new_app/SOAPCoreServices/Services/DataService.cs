using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SOAPCoreServices.Interfaces;
using SOAPCoreServices.Models;

namespace SOAPCoreServices.Services
{
    public class DataService : IDataService
    {
        private readonly ILogger<DataService> _logger;

        public DataService(ILogger<DataService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns a Hello World message
        /// </summary>
        public Task<string> HelloWorldAsync()
        {
            _logger.LogInformation("HelloWorld method called");
            return Task.FromResult("Hello World");
        }

        /// <summary>
        /// Returns a personalized greeting
        /// </summary>
        public Task<string> GetDataAsync(string name)
        {
            _logger.LogInformation($"GetData method called with name: {name}");
            return Task.FromResult($"Hello {name}, this is a simple SOAP web service response.");
        }

        /// <summary>
        /// Returns a sample dataset
        /// </summary>
        public Task<DataSet> GetDataSetAsync()
        {
            _logger.LogInformation("GetDataSet method called");
            
            try
            {
                var ds = new DataSet("SampleDataSet");
                var dt = new DataTable("SampleTable");
                dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                // Adding some sample data
                dt.Rows.Add(1, "Alice");
                dt.Rows.Add(2, "Bob");
                ds.Tables.Add(dt);
                
                return Task.FromResult(ds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating dataset in GetDataSet method");
                throw;
            }
        }

        /// <summary>
        /// Returns a report based on the input parameters
        /// </summary>
        public Task<DataSet> GetReportAsync(ReportInput reportInput)
        {
            _logger.LogInformation($"GetReport method called for report: {reportInput.ReportName}");
            
            if (string.IsNullOrEmpty(reportInput.ReportName))
            {
                _logger.LogWarning("Report name is empty");
            }
            
            return GetDataSetAsync();
        }
        
        #region Legacy synchronous methods
        
        public string HelloWorld()
        {
            return HelloWorldAsync().GetAwaiter().GetResult();
        }

        public string GetData(string name)
        {
            return GetDataAsync(name).GetAwaiter().GetResult();
        }

        public DataSet GetDataSet()
        {
            return GetDataSetAsync().GetAwaiter().GetResult();
        }

        public DataSet GetReport(ReportInput reportInput)
        {
            return GetReportAsync(reportInput).GetAwaiter().GetResult();
        }
        
        #endregion
    }
}