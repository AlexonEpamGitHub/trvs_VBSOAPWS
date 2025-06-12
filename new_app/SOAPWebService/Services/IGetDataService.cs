using System.Data;
using System.ServiceModel;
using SOAPWebService.Models;

namespace SOAPWebService.Services
{
    /// <summary>
    /// Service contract interface for SOAP web service operations.
    /// Migrated from legacy VB.NET WebService to modern .NET 8 ServiceContract with async patterns.
    /// </summary>
    [ServiceContract(Namespace = "http://tempuri.org/")]
    public interface IGetDataService
    {
        /// <summary>
        /// Returns a simple "Hello World" greeting asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>Task containing Hello World string</returns>
        [OperationContract(Action = "http://tempuri.org/HelloWorldAsync", ReplyAction = "http://tempuri.org/HelloWorldAsyncResponse")]
        Task<string> HelloWorldAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a personalized greeting message asynchronously.
        /// </summary>
        /// <param name="name">Name to include in the greeting</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>Task containing personalized greeting string</returns>
        [OperationContract(Action = "http://tempuri.org/GetDataAsync", ReplyAction = "http://tempuri.org/GetDataAsyncResponse")]
        Task<string> GetDataAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a sample ResponseDto with test data asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>Task containing ResponseDto with sample data</returns>
        [OperationContract(Action = "http://tempuri.org/GetDataSetAsync", ReplyAction = "http://tempuri.org/GetDataSetAsyncResponse")]
        Task<ResponseDto> GetDataSetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns report data based on the provided input parameters asynchronously.
        /// </summary>
        /// <param name="reportInput">Report input parameters</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>Task containing ReportResponseDto with report data</returns>
        [OperationContract(Action = "http://tempuri.org/GetReportAsync", ReplyAction = "http://tempuri.org/GetReportAsyncResponse")]
        Task<ReportResponseDto> GetReportAsync(ReportInput reportInput, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Data transfer object for service responses containing structured data.
    /// </summary>
    [DataContract(Namespace = "http://tempuri.org/")]
    public class ResponseDto
    {
        /// <summary>
        /// Gets or sets the success status of the operation.
        /// </summary>
        [DataMember(Order = 1)]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the response message.
        /// </summary>
        [DataMember(Order = 2)]
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the collection of data items.
        /// </summary>
        [DataMember(Order = 3)]
        public List<DataItem>? Data { get; set; }

        /// <summary>
        /// Gets or sets the total count of items.
        /// </summary>
        [DataMember(Order = 4)]
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the response.
        /// </summary>
        [DataMember(Order = 5)]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Initializes a new instance of the ResponseDto class.
        /// </summary>
        public ResponseDto()
        {
            Success = false;
            Data = new List<DataItem>();
            Timestamp = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Data transfer object for report service responses containing structured report data.
    /// </summary>
    [DataContract(Namespace = "http://tempuri.org/")]
    public class ReportResponseDto
    {
        /// <summary>
        /// Gets or sets the success status of the report operation.
        /// </summary>
        [DataMember(Order = 1)]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the report response message.
        /// </summary>
        [DataMember(Order = 2)]
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the collection of report data items.
        /// </summary>
        [DataMember(Order = 3)]
        public List<ReportDataItem>? ReportData { get; set; }

        /// <summary>
        /// Gets or sets the total count of report items.
        /// </summary>
        [DataMember(Order = 4)]
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the report response.
        /// </summary>
        [DataMember(Order = 5)]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the report metadata.
        /// </summary>
        [DataMember(Order = 6)]
        public ReportMetadata? Metadata { get; set; }

        /// <summary>
        /// Initializes a new instance of the ReportResponseDto class.
        /// </summary>
        public ReportResponseDto()
        {
            Success = false;
            ReportData = new List<ReportDataItem>();
            Timestamp = DateTime.UtcNow;
            Metadata = new ReportMetadata();
        }
    }

    /// <summary>
    /// Data transfer object representing a single data item.
    /// </summary>
    [DataContract(Namespace = "http://tempuri.org/")]
    public class DataItem
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        [DataMember(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name or title.
        /// </summary>
        [DataMember(Order = 2)]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the value associated with the item.
        /// </summary>
        [DataMember(Order = 3)]
        public string? Value { get; set; }

        /// <summary>
        /// Gets or sets the description of the item.
        /// </summary>
        [DataMember(Order = 4)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the item.
        /// </summary>
        [DataMember(Order = 5)]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets whether the item is active.
        /// </summary>
        [DataMember(Order = 6)]
        public bool IsActive { get; set; }

        /// <summary>
        /// Initializes a new instance of the DataItem class.
        /// </summary>
        public DataItem()
        {
            CreatedDate = DateTime.UtcNow;
            IsActive = true;
        }
    }

    /// <summary>
    /// Data transfer object representing a single report data item.
    /// </summary>
    [DataContract(Namespace = "http://tempuri.org/")]
    public class ReportDataItem
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        [DataMember(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the report item name.
        /// </summary>
        [DataMember(Order = 2)]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the report value.
        /// </summary>
        [DataMember(Order = 3)]
        public string? Value { get; set; }

        /// <summary>
        /// Gets or sets the report category.
        /// </summary>
        [DataMember(Order = 4)]
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the numeric value for calculations.
        /// </summary>
        [DataMember(Order = 5)]
        public decimal NumericValue { get; set; }

        /// <summary>
        /// Gets or sets the report date.
        /// </summary>
        [DataMember(Order = 6)]
        public DateTime ReportDate { get; set; }

        /// <summary>
        /// Gets or sets whether the report item is valid.
        /// </summary>
        [DataMember(Order = 7)]
        public bool IsValid { get; set; }

        /// <summary>
        /// Initializes a new instance of the ReportDataItem class.
        /// </summary>
        public ReportDataItem()
        {
            ReportDate = DateTime.UtcNow;
            IsValid = true;
            NumericValue = 0;
        }
    }

    /// <summary>
    /// Data transfer object representing report metadata.
    /// </summary>
    [DataContract(Namespace = "http://tempuri.org/")]
    public class ReportMetadata
    {
        /// <summary>
        /// Gets or sets the report title.
        /// </summary>
        [DataMember(Order = 1)]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the report description.
        /// </summary>
        [DataMember(Order = 2)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the report generated date.
        /// </summary>
        [DataMember(Order = 3)]
        public DateTime GeneratedDate { get; set; }

        /// <summary>
        /// Gets or sets the report version.
        /// </summary>
        [DataMember(Order = 4)]
        public string? Version { get; set; }

        /// <summary>
        /// Gets or sets the report format.
        /// </summary>
        [DataMember(Order = 5)]
        public string? Format { get; set; }

        /// <summary>
        /// Initializes a new instance of the ReportMetadata class.
        /// </summary>
        public ReportMetadata()
        {
            GeneratedDate = DateTime.UtcNow;
            Version = "1.0";
            Format = "Standard";
        }
    }
}