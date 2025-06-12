using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SOAPWebService.Models;

/// <summary>
/// Response data transfer object for SOAP operations
/// </summary>
[DataContract]
[XmlType("Response")]
public record ResponseDto : IValidatableObject
{
    /// <summary>
    /// Collection of data items
    /// </summary>
    [DataMember]
    [XmlArray("Items")]
    [XmlArrayItem("DataItem", Type = typeof(DataItem))]
    public List<DataItem> Items { get; init; } = [];

    /// <summary>
    /// Response message
    /// </summary>
    [DataMember]
    [XmlElement("Message")]
    [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Timestamp of the response
    /// </summary>
    [DataMember]
    [XmlElement("Timestamp")]
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Success status of the operation
    /// </summary>
    [DataMember]
    [XmlElement("Success")]
    public bool Success { get; init; } = true;

    /// <summary>
    /// Response metadata containing additional information
    /// </summary>
    [DataMember]
    [XmlElement("Metadata")]
    public ResponseMetadata? Metadata { get; init; }

    /// <summary>
    /// Collection of validation errors if any occurred
    /// </summary>
    [DataMember]
    [XmlArray("ValidationErrors")]
    [XmlArrayItem("Error", Type = typeof(string))]
    public List<string> ValidationErrors { get; init; } = [];

    /// <summary>
    /// Operation identifier for tracking
    /// </summary>
    [DataMember]
    [XmlElement("OperationId")]
    [StringLength(50, ErrorMessage = "OperationId cannot exceed 50 characters")]
    public string? OperationId { get; init; }

    /// <summary>
    /// Validates the response object
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        
        if (Items is null)
        {
            results.Add(new ValidationResult("Items collection cannot be null", [nameof(Items)]));
        }
        else
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var itemResults = new List<ValidationResult>();
                var itemContext = new ValidationContext(Items[i]);
                Validator.TryValidateObject(Items[i], itemContext, itemResults, true);
                
                foreach (var itemResult in itemResults)
                {
                    results.Add(new ValidationResult(
                        $"Item {i}: {itemResult.ErrorMessage}",
                        itemResult.MemberNames.Select(m => $"Items[{i}].{m}")
                    ));
                }
            }
        }

        if (!string.IsNullOrEmpty(OperationId) && OperationId.Length > 50)
        {
            results.Add(new ValidationResult("OperationId cannot exceed 50 characters", [nameof(OperationId)]));
        }
        
        return results;
    }
}

/// <summary>
/// Individual data item replacing DataRow functionality
/// </summary>
[DataContract]
[XmlType("DataItem")]
public record DataItem : IValidatableObject
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    [DataMember]
    [XmlElement("Id")]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be a positive number")]
    public int Id { get; init; }

    /// <summary>
    /// Item name
    /// </summary>
    [DataMember]
    [XmlElement("Name")]
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Item description
    /// </summary>
    [DataMember]
    [XmlElement("Description")]
    [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
    public string? Description { get; init; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    [DataMember]
    [XmlElement("CreatedAt")]
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Item value represented as object for flexible data types
    /// </summary>
    [DataMember]
    [XmlElement("Value")]
    public object? Value { get; init; }

    /// <summary>
    /// Data type of the value
    /// </summary>
    [DataMember]
    [XmlElement("DataType")]
    [StringLength(50, ErrorMessage = "DataType cannot exceed 50 characters")]
    public string? DataType { get; init; }

    /// <summary>
    /// Indicates if the item is active
    /// </summary>
    [DataMember]
    [XmlElement("IsActive")]
    public bool IsActive { get; init; } = true;

    /// <summary>
    /// Item category or classification
    /// </summary>
    [DataMember]
    [XmlElement("Category")]
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string? Category { get; init; }

    /// <summary>
    /// Item priority level
    /// </summary>
    [DataMember]
    [XmlElement("Priority")]
    [Range(1, 10, ErrorMessage = "Priority must be between 1 and 10")]
    public int Priority { get; init; } = 5;

    /// <summary>
    /// Last modified timestamp
    /// </summary>
    [DataMember]
    [XmlElement("LastModified")]
    public DateTime? LastModified { get; init; }

    /// <summary>
    /// User who last modified the item
    /// </summary>
    [DataMember]
    [XmlElement("ModifiedBy")]
    [StringLength(100, ErrorMessage = "ModifiedBy cannot exceed 100 characters")]
    public string? ModifiedBy { get; init; }

    /// <summary>
    /// Additional properties as key-value pairs
    /// </summary>
    [DataMember]
    [XmlArray("Properties")]
    [XmlArrayItem("Property", Type = typeof(KeyValueItem))]
    public List<KeyValueItem> Properties { get; init; } = [];

    /// <summary>
    /// Tags associated with the item
    /// </summary>
    [DataMember]
    [XmlArray("Tags")]
    [XmlArrayItem("Tag", Type = typeof(string))]
    public List<string> Tags { get; init; } = [];

    /// <summary>
    /// Validates the data item
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        
        if (string.IsNullOrWhiteSpace(Name))
        {
            results.Add(new ValidationResult("Name cannot be empty or whitespace", [nameof(Name)]));
        }

        if (Id <= 0)
        {
            results.Add(new ValidationResult("Id must be greater than zero", [nameof(Id)]));
        }

        if (!string.IsNullOrEmpty(DataType) && Value is not null)
        {
            var isValidType = DataType.ToLowerInvariant() switch
            {
                "string" => Value is string,
                "int" or "integer" => Value is int,
                "decimal" or "double" => Value is decimal or double,
                "bool" or "boolean" => Value is bool,
                "datetime" => Value is DateTime,
                _ => true // Allow unknown types
            };

            if (!isValidType)
            {
                results.Add(new ValidationResult($"Value type does not match specified DataType '{DataType}'", [nameof(Value), nameof(DataType)]));
            }
        }

        if (Priority < 1 || Priority > 10)
        {
            results.Add(new ValidationResult("Priority must be between 1 and 10", [nameof(Priority)]));
        }
        
        return results;
    }
}

/// <summary>
/// Key-value pair item for flexible property storage
/// </summary>
[DataContract]
[XmlType("KeyValueItem")]
public record KeyValueItem
{
    /// <summary>
    /// Property key
    /// </summary>
    [DataMember]
    [XmlElement("Key")]
    [Required(ErrorMessage = "Key is required")]
    [StringLength(100, ErrorMessage = "Key cannot exceed 100 characters")]
    public string Key { get; init; } = string.Empty;

    /// <summary>
    /// Property value
    /// </summary>
    [DataMember]
    [XmlElement("Value")]
    public object? Value { get; init; }

    /// <summary>
    /// Value type for serialization purposes
    /// </summary>
    [DataMember]
    [XmlElement("ValueType")]
    [StringLength(50, ErrorMessage = "ValueType cannot exceed 50 characters")]
    public string? ValueType { get; init; }
}

/// <summary>
/// Response metadata containing additional information about the response
/// </summary>
[DataContract]
[XmlType("ResponseMetadata")]
public record ResponseMetadata
{
    /// <summary>
    /// Total number of items in the complete dataset
    /// </summary>
    [DataMember]
    [XmlElement("TotalCount")]
    [Range(0, int.MaxValue, ErrorMessage = "TotalCount must be non-negative")]
    public int TotalCount { get; init; }

    /// <summary>
    /// Current page number for paginated responses
    /// </summary>
    [DataMember]
    [XmlElement("PageNumber")]
    [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be positive")]
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    [DataMember]
    [XmlElement("PageSize")]
    [Range(1, 1000, ErrorMessage = "PageSize must be between 1 and 1000")]
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Total number of pages available
    /// </summary>
    [DataMember]
    [XmlElement("TotalPages")]
    [Range(0, int.MaxValue, ErrorMessage = "TotalPages must be non-negative")]
    public int TotalPages { get; init; }

    /// <summary>
    /// Indicates if there are more pages available
    /// </summary>
    [DataMember]
    [XmlElement("HasNextPage")]
    public bool HasNextPage { get; init; }

    /// <summary>
    /// Indicates if there are previous pages available
    /// </summary>
    [DataMember]
    [XmlElement("HasPreviousPage")]
    public bool HasPreviousPage { get; init; }

    /// <summary>
    /// Processing duration in milliseconds
    /// </summary>
    [DataMember]
    [XmlElement("ProcessingTimeMs")]
    [Range(0, long.MaxValue, ErrorMessage = "ProcessingTimeMs must be non-negative")]
    public long ProcessingTimeMs { get; init; }

    /// <summary>
    /// Source system or service that generated the response
    /// </summary>
    [DataMember]
    [XmlElement("Source")]
    [StringLength(100, ErrorMessage = "Source cannot exceed 100 characters")]
    public string? Source { get; init; }

    /// <summary>
    /// Version of the API or service
    /// </summary>
    [DataMember]
    [XmlElement("Version")]
    [StringLength(20, ErrorMessage = "Version cannot exceed 20 characters")]
    public string? Version { get; init; }

    /// <summary>
    /// Server or instance identifier
    /// </summary>
    [DataMember]
    [XmlElement("ServerId")]
    [StringLength(50, ErrorMessage = "ServerId cannot exceed 50 characters")]
    public string? ServerId { get; init; }

    /// <summary>
    /// Request correlation identifier
    /// </summary>
    [DataMember]
    [XmlElement("CorrelationId")]
    [StringLength(100, ErrorMessage = "CorrelationId cannot exceed 100 characters")]
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Additional metadata as key-value pairs
    /// </summary>
    [DataMember]
    [XmlArray("AdditionalData")]
    [XmlArrayItem("Data", Type = typeof(KeyValueItem))]
    public List<KeyValueItem> AdditionalData { get; init; } = [];
}

/// <summary>
/// Specialized response DTO for report operations
/// </summary>
[DataContract]
[XmlType("ReportResponse")]
public record ReportResponseDto : ResponseDto
{
    /// <summary>
    /// Report identifier
    /// </summary>
    [DataMember]
    [XmlElement("ReportId")]
    [Required(ErrorMessage = "ReportId is required")]
    [StringLength(50, ErrorMessage = "ReportId cannot exceed 50 characters")]
    public string ReportId { get; init; } = string.Empty;

    /// <summary>
    /// Report name or title
    /// </summary>
    [DataMember]
    [XmlElement("ReportName")]
    [Required(ErrorMessage = "ReportName is required")]
    [StringLength(200, ErrorMessage = "ReportName cannot exceed 200 characters")]
    public string ReportName { get; init; } = string.Empty;

    /// <summary>
    /// Report generation timestamp
    /// </summary>
    [DataMember]
    [XmlElement("GeneratedAt")]
    public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// User who requested the report
    /// </summary>
    [DataMember]
    [XmlElement("RequestedBy")]
    [StringLength(100, ErrorMessage = "RequestedBy cannot exceed 100 characters")]
    public string? RequestedBy { get; init; }

    /// <summary>
    /// Report format (PDF, Excel, CSV, etc.)
    /// </summary>
    [DataMember]
    [XmlElement("Format")]
    [StringLength(20, ErrorMessage = "Format cannot exceed 20 characters")]
    public string? Format { get; init; }

    /// <summary>
    /// Report type or category
    /// </summary>
    [DataMember]
    [XmlElement("ReportType")]
    [StringLength(50, ErrorMessage = "ReportType cannot exceed 50 characters")]
    public string? ReportType { get; init; }

    /// <summary>
    /// Report description
    /// </summary>
    [DataMember]
    [XmlElement("Description")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; init; }

    /// <summary>
    /// Report parameters used for generation
    /// </summary>
    [DataMember]
    [XmlArray("Parameters")]
    [XmlArrayItem("Parameter", Type = typeof(KeyValueItem))]
    public List<KeyValueItem> Parameters { get; init; } = [];

    /// <summary>
    /// Collection of report sections or categories
    /// </summary>
    [DataMember]
    [XmlArray("Sections")]
    [XmlArrayItem("Section", Type = typeof(ReportSection))]
    public List<ReportSection> Sections { get; init; } = [];

    /// <summary>
    /// Report summary information
    /// </summary>
    [DataMember]
    [XmlElement("Summary")]
    public ReportSummary? Summary { get; init; }

    /// <summary>
    /// Report execution status
    /// </summary>
    [DataMember]
    [XmlElement("ExecutionStatus")]
    [StringLength(20, ErrorMessage = "ExecutionStatus cannot exceed 20 characters")]
    public string ExecutionStatus { get; init; } = "Completed";

    /// <summary>
    /// Report file size in bytes if applicable
    /// </summary>
    [DataMember]
    [XmlElement("FileSizeBytes")]
    [Range(0, long.MaxValue, ErrorMessage = "FileSizeBytes must be non-negative")]
    public long? FileSizeBytes { get; init; }

    /// <summary>
    /// Report expiration date
    /// </summary>
    [DataMember]
    [XmlElement("ExpiresAt")]
    public DateTime? ExpiresAt { get; init; }
}

/// <summary>
/// Represents a section within a report
/// </summary>
[DataContract]
[XmlType("ReportSection")]
public record ReportSection
{
    /// <summary>
    /// Section identifier
    /// </summary>
    [DataMember]
    [XmlElement("Id")]
    [Required(ErrorMessage = "Section Id is required")]
    [StringLength(50, ErrorMessage = "Section Id cannot exceed 50 characters")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Section title
    /// </summary>
    [DataMember]
    [XmlElement("Title")]
    [Required(ErrorMessage = "Section Title is required")]
    [StringLength(200, ErrorMessage = "Section Title cannot exceed 200 characters")]
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Section description
    /// </summary>
    [DataMember]
    [XmlElement("Description")]
    [StringLength(500, ErrorMessage = "Section Description cannot exceed 500 characters")]
    public string? Description { get; init; }

    /// <summary>
    /// Section order for display
    /// </summary>
    [DataMember]
    [XmlElement("Order")]
    [Range(0, int.MaxValue, ErrorMessage = "Order must be non-negative")]
    public int Order { get; init; }

    /// <summary>
    /// Section type or category
    /// </summary>
    [DataMember]
    [XmlElement("SectionType")]
    [StringLength(50, ErrorMessage = "SectionType cannot exceed 50 characters")]
    public string? SectionType { get; init; }

    /// <summary>
    /// Data items in this section
    /// </summary>
    [DataMember]
    [XmlArray("Data")]
    [XmlArrayItem("DataItem", Type = typeof(DataItem))]
    public List<DataItem> Data { get; init; } = [];

    /// <summary>
    /// Chart or visualization data if applicable
    /// </summary>
    [DataMember]
    [XmlArray("Charts")]
    [XmlArrayItem("Chart", Type = typeof(ChartData))]
    public List<ChartData> Charts { get; init; } = [];

    /// <summary>
    /// Section-specific metadata
    /// </summary>
    [DataMember]
    [XmlArray("Metadata")]
    [XmlArrayItem("Meta", Type = typeof(KeyValueItem))]
    public List<KeyValueItem> Metadata { get; init; } = [];

    /// <summary>
    /// Indicates if the section is visible
    /// </summary>
    [DataMember]
    [XmlElement("IsVisible")]
    public bool IsVisible { get; init; } = true;
}

/// <summary>
/// Chart or visualization data for reports
/// </summary>
[DataContract]
[XmlType("ChartData")]
public record ChartData
{
    /// <summary>
    /// Chart identifier
    /// </summary>
    [DataMember]
    [XmlElement("Id")]
    [Required(ErrorMessage = "Chart Id is required")]
    [StringLength(50, ErrorMessage = "Chart Id cannot exceed 50 characters")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Chart title
    /// </summary>
    [DataMember]
    [XmlElement("Title")]
    [Required(ErrorMessage = "Chart Title is required")]
    [StringLength(200, ErrorMessage = "Chart Title cannot exceed 200 characters")]
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Chart type (Bar, Line, Pie, etc.)
    /// </summary>
    [DataMember]
    [XmlElement("ChartType")]
    [Required(ErrorMessage = "ChartType is required")]
    [StringLength(50, ErrorMessage = "ChartType cannot exceed 50 characters")]
    public string ChartType { get; init; } = string.Empty;

    /// <summary>
    /// Chart data series
    /// </summary>
    [DataMember]
    [XmlArray("Series")]
    [XmlArrayItem("DataSeries", Type = typeof(DataSeries))]
    public List<DataSeries> Series { get; init; } = [];

    /// <summary>
    /// X-axis labels
    /// </summary>
    [DataMember]
    [XmlArray("XAxisLabels")]
    [XmlArrayItem("Label", Type = typeof(string))]
    public List<string> XAxisLabels { get; init; } = [];

    /// <summary>
    /// Chart configuration options
    /// </summary>
    [DataMember]
    [XmlArray("Options")]
    [XmlArrayItem("Option", Type = typeof(KeyValueItem))]
    public List<KeyValueItem> Options { get; init; } = [];
}

/// <summary>
/// Data series for chart visualization
/// </summary>
[DataContract]
[XmlType("DataSeries")]
public record DataSeries
{
    /// <summary>
    /// Series name
    /// </summary>
    [DataMember]
    [XmlElement("Name")]
    [Required(ErrorMessage = "Series Name is required")]
    [StringLength(100, ErrorMessage = "Series Name cannot exceed 100 characters")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Series data values
    /// </summary>
    [DataMember]
    [XmlArray("Values")]
    [XmlArrayItem("Value", Type = typeof(decimal))]
    public List<decimal> Values { get; init; } = [];

    /// <summary>
    /// Series color for visualization
    /// </summary>
    [DataMember]
    [XmlElement("Color")]
    [StringLength(20, ErrorMessage = "Color cannot exceed 20 characters")]
    public string? Color { get; init; }

    /// <summary>
    /// Series type (if different from chart type)
    /// </summary>
    [DataMember]
    [XmlElement("SeriesType")]
    [StringLength(50, ErrorMessage = "SeriesType cannot exceed 50 characters")]
    public string? SeriesType { get; init; }
}

/// <summary>
/// Report summary containing aggregate information
/// </summary>
[DataContract]
[XmlType("ReportSummary")]
public record ReportSummary
{
    /// <summary>
    /// Total number of records processed
    /// </summary>
    [DataMember]
    [XmlElement("TotalRecords")]
    [Range(0, int.MaxValue, ErrorMessage = "TotalRecords must be non-negative")]
    public int TotalRecords { get; init; }

    /// <summary>
    /// Number of successful operations
    /// </summary>
    [DataMember]
    [XmlElement("SuccessCount")]
    [Range(0, int.MaxValue, ErrorMessage = "SuccessCount must be non-negative")]
    public int SuccessCount { get; init; }

    /// <summary>
    /// Number of failed operations
    /// </summary>
    [DataMember]
    [XmlElement("ErrorCount")]
    [Range(0, int.MaxValue, ErrorMessage = "ErrorCount must be non-negative")]
    public int ErrorCount { get; init; }

    /// <summary>
    /// Number of warnings encountered
    /// </summary>
    [DataMember]
    [XmlElement("WarningCount")]
    [Range(0, int.MaxValue, ErrorMessage = "WarningCount must be non-negative")]
    public int WarningCount { get; init; }

    /// <summary>
    /// Collection of warnings encountered during processing
    /// </summary>
    [DataMember]
    [XmlArray("Warnings")]
    [XmlArrayItem("Warning", Type = typeof(string))]
    public List<string> Warnings { get; init; } = [];

    /// <summary>
    /// Collection of errors encountered during processing
    /// </summary>
    [DataMember]
    [XmlArray("Errors")]
    [XmlArrayItem("Error", Type = typeof(string))]
    public List<string> Errors { get; init; } = [];

    /// <summary>
    /// Key performance indicators or metrics
    /// </summary>
    [DataMember]
    [XmlArray("Metrics")]
    [XmlArrayItem("Metric", Type = typeof(MetricItem))]
    public List<MetricItem> Metrics { get; init; } = [];

    /// <summary>
    /// Summary statistics
    /// </summary>
    [DataMember]
    [XmlArray("Statistics")]
    [XmlArrayItem("Statistic", Type = typeof(StatisticItem))]
    public List<StatisticItem> Statistics { get; init; } = [];

    /// <summary>
    /// Overall completion percentage
    /// </summary>
    [DataMember]
    [XmlElement("CompletionPercentage")]
    [Range(0.0, 100.0, ErrorMessage = "CompletionPercentage must be between 0 and 100")]
    public decimal CompletionPercentage { get; init; }
}

/// <summary>
/// Metric item for report summaries
/// </summary>
[DataContract]
[XmlType("MetricItem")]
public record MetricItem
{
    /// <summary>
    /// Metric name
    /// </summary>
    [DataMember]
    [XmlElement("Name")]
    [Required(ErrorMessage = "Metric Name is required")]
    [StringLength(100, ErrorMessage = "Metric Name cannot exceed 100 characters")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Metric value
    /// </summary>
    [DataMember]
    [XmlElement("Value")]
    public decimal Value { get; init; }

    /// <summary>
    /// Metric unit of measurement
    /// </summary>
    [DataMember]
    [XmlElement("Unit")]
    [StringLength(20, ErrorMessage = "Unit cannot exceed 20 characters")]
    public string? Unit { get; init; }

    /// <summary>
    /// Metric description
    /// </summary>
    [DataMember]
    [XmlElement("Description")]
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    public string? Description { get; init; }
}

/// <summary>
/// Statistic item for report summaries
/// </summary>
[DataContract]
[XmlType("StatisticItem")]
public record StatisticItem
{
    /// <summary>
    /// Statistic name
    /// </summary>
    [DataMember]
    [XmlElement("Name")]
    [Required(ErrorMessage = "Statistic Name is required")]
    [StringLength(100, ErrorMessage = "Statistic Name cannot exceed 100 characters")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Statistic value
    /// </summary>
    [DataMember]
    [XmlElement("Value")]
    public decimal Value { get; init; }

    /// <summary>
    /// Statistic category
    /// </summary>
    [DataMember]
    [XmlElement("Category")]
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string? Category { get; init; }

    /// <summary>
    /// Calculation method used
    /// </summary>
    [DataMember]
    [XmlElement("CalculationMethod")]
    [StringLength(50, ErrorMessage = "CalculationMethod cannot exceed 50 characters")]
    public string? CalculationMethod { get; init; }
}