using System;
using System.Data;
using System.ServiceModel;
using System.Threading.Tasks;

namespace SOAPCoreServices.Interfaces
{
    [ServiceContract(Namespace = "http://tempuri.org/")]
    public interface IDataService
    {
        /// <summary>
        /// Basic Hello World operation
        /// </summary>
        [OperationContract]
        Task<string> HelloWorldAsync();

        /// <summary>
        /// Returns a greeting message with the provided name
        /// </summary>
        /// <param name="name">Name to include in greeting</param>
        [OperationContract]
        Task<string> GetDataAsync(string name);

        /// <summary>
        /// Returns a sample dataset
        /// </summary>
        [OperationContract]
        Task<DataSet> GetDataSetAsync();

        /// <summary>
        /// Returns a report based on the input parameters
        /// </summary>
        /// <param name="reportInput">Report input parameters</param>
        [OperationContract]
        Task<DataSet> GetReportAsync(Models.ReportInput reportInput);
    }
}