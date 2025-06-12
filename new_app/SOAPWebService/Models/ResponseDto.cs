using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SOAPWebService.Models;

/// <summary>
/// Base response data transfer object for SOAP operations
/// </summary>
[DataContract]
[XmlType("Response")]
public record ResponseDto : IValidatableObject
{
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
    /// Response message
    /// </summary>
    [DataMember]
    [XmlElement("Message")]
    [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Operation identifier for tracking
    /// </summary>
    [DataMember]
    [XmlElement("OperationId")]
    [StringLength(50, ErrorMessage = "OperationId cannot exceed 50 characters")]
    public string? OperationId { get; init; }

    /// <summary>
    /// Collection of validation errors if any occurred
    /// </summary>
    [DataMember]
    [XmlArray("ValidationErrors")]
    [XmlArrayItem("Error", Type = typeof(string))]
    public List<string> ValidationErrors { get; init; } = [];

    /// <summary>
    /// Validates the response object
    /// </summary>
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (!string.IsNullOrEmpty(OperationId) && OperationId.Length > 50)
        {
            results.Add(new ValidationResult("OperationId cannot exceed 50 characters", [nameof(OperationId)]));
        }

        if (!string.IsNullOrEmpty(Message) && Message.Length > 500)
        {
            results.Add(new ValidationResult("Message cannot exceed 500 characters", [nameof(Message)]));
        }

        return results;
    }
}

/// <summary>
/// Response data transfer object for sample data operations
/// </summary>
[DataContract]
[XmlType("SampleDataResponse")]
public record SampleDataResponse : ResponseDto
{
    /// <summary>
    /// Collection of sample data items
    /// </summary>
    [DataMember]
    [XmlArray("SampleItems")]
    [XmlArrayItem("SampleDataItem", Type = typeof(SampleDataItem))]
    public List<SampleDataItem> SampleItems { get; init; } = [];

    /// <summary>
    /// Total count of available sample items
    /// </summary>
    [DataMember]
    [XmlElement("TotalCount")]
    [Range(0, int.MaxValue, ErrorMessage = "TotalCount must be non-negative")]
    public int TotalCount { get; init; }

    /// <summary>
    /// Sample data collection metadata
    /// </summary>
    [DataMember]
    [XmlElement("CollectionName")]
    [StringLength(100, ErrorMessage = "CollectionName cannot exceed 100 characters")]
    public string? CollectionName { get; init; }

    /// <summary>
    /// Data source identifier
    /// </summary>
    [DataMember]
    [XmlElement("DataSource")]
    [StringLength(100, ErrorMessage = "DataSource cannot exceed 100 characters")]
    public string? DataSource { get; init; }

    /// <summary>
    /// Last updated timestamp for the sample data
    /// </summary>
    [DataMember]
    [XmlElement("LastUpdated")]
    public DateTime? LastUpdated { get; init; }

    /// <summary>
    /// Pagination information
    /// </summary>
    [DataMember]
    [XmlElement("PageNumber")]
    [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be positive")]
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Page size for pagination
    /// </summary>
    [DataMember]
    [XmlElement("PageSize")]
    [Range(1, 1000, ErrorMessage = "PageSize must be between 1 and 1000")]
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Indicates if more pages are available
    /// </summary>
    [DataMember]
    [XmlElement("HasMorePages")]
    public bool HasMorePages { get; init; }

    /// <summary>
    /// Sample data version
    /// </summary>
    [DataMember]
    [XmlElement("Version")]
    [StringLength(20, ErrorMessage = "Version cannot exceed 20 characters")]
    public string? Version { get; init; }

    /// <summary>
    /// Validates the sample data response
    /// </summary>
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = base.Validate(validationContext).ToList();

        if (SampleItems is null)
        {
            results.Add(new ValidationResult("SampleItems collection cannot be null", [nameof(SampleItems)]));
        }
        else
        {
            for (int i = 0; i < SampleItems.Count; i++)
            {
                var itemResults = new List<ValidationResult>();
                var itemContext = new ValidationContext(SampleItems[i]);
                Validator.TryValidateObject(SampleItems[i], itemContext, itemResults, true);

                foreach (var itemResult in itemResults)
                {
                    results.Add(new ValidationResult(
                        $"SampleItem {i}: {itemResult.ErrorMessage}",
                        itemResult.MemberNames.Select(m => $"SampleItems[{i}].{m}")
                    ));
                }
            }
        }

        if (TotalCount < 0)
        {
            results.Add(new ValidationResult("TotalCount must be non-negative", [nameof(TotalCount)]));
        }

        if (PageNumber < 1)
        {
            results.Add(new ValidationResult("PageNumber must be positive", [nameof(PageNumber)]));
        }

        if (PageSize < 1 || PageSize > 1000)
        {
            results.Add(new ValidationResult("PageSize must be between 1 and 1000", [nameof(PageSize)]));
        }

        return results;
    }
}

/// <summary>
/// Response data transfer object for report operations
/// </summary>
[DataContract]
[XmlType("ReportDataResponse")]
public record ReportDataResponse : ResponseDto
{
    /// <summary>
    /// Collection of report data items
    /// </summary>
    [DataMember]
    [XmlArray("ReportItems")]
    [XmlArrayItem("ReportDataItem", Type = typeof(ReportDataItem))]
    public List<ReportDataItem> ReportItems { get; init; } = [];

    /// <summary>
    /// Report metadata containing additional information
    /// </summary>
    [DataMember]
    [XmlElement("ReportMetadata")]
    public ReportMetadata? ReportMetadata { get; init; }

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
    /// Report execution status
    /// </summary>
    [DataMember]
    [XmlElement("ExecutionStatus")]
    [StringLength(20, ErrorMessage = "ExecutionStatus cannot exceed 20 characters")]
    public string ExecutionStatus { get; init; } = "Completed";

    /// <summary>
    /// Total number of report items
    /// </summary>
    [DataMember]
    [XmlElement("TotalItems")]
    [Range(0, int.MaxValue, ErrorMessage = "TotalItems must be non-negative")]
    public int TotalItems { get; init; }

    /// <summary>
    /// Processing duration in milliseconds
    /// </summary>
    [DataMember]
    [XmlElement("ProcessingTimeMs")]
    [Range(0, long.MaxValue, ErrorMessage = "ProcessingTimeMs must be non-negative")]
    public long ProcessingTimeMs { get; init; }

    /// <summary>
    /// Report format (PDF, Excel, CSV, etc.)
    /// </summary>
    [DataMember]
    [XmlElement("Format")]
    [StringLength(20, ErrorMessage = "Format cannot exceed 20 characters")]
    public string? Format { get; init; }

    /// <summary>
    /// Report expiration date
    /// </summary>
    [DataMember]
    [XmlElement("ExpiresAt")]
    public DateTime? ExpiresAt { get; init; }

    /// <summary>
    /// Validates the report data response
    /// </summary>
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = base.Validate(validationContext).ToList();

        if (string.IsNullOrWhiteSpace(ReportId))
        {
            results.Add(new ValidationResult("ReportId is required", [nameof(ReportId)]));
        }

        if (string.IsNullOrWhiteSpace(ReportName))
        {
            results.Add(new ValidationResult("ReportName is required", [nameof(ReportName)]));
        }

        if (ReportItems is null)
        {
            results.Add(new ValidationResult("ReportItems collection cannot be null", [nameof(ReportItems)]));
        }
        else
        {
            for (int i = 0; i < ReportItems.Count; i++)
            {
                var itemResults = new List<ValidationResult>();
                var itemContext = new ValidationContext(ReportItems[i]);
                Validator.TryValidateObject(ReportItems[i], itemContext, itemResults, true);

                foreach (var itemResult in itemResults)
                {
                    results.Add(new ValidationResult(
                        $"ReportItem {i}: {itemResult.ErrorMessage}",
                        itemResult.MemberNames.Select(m => $"ReportItems[{i}].{m}")
                    ));
                }
            }
        }

        if (TotalItems < 0)
        {
            results.Add(new ValidationResult("TotalItems must be non-negative", [nameof(TotalItems)]));
        }

        if (ProcessingTimeMs < 0)
        {
            results.Add(new ValidationResult("ProcessingTimeMs must be non-negative", [nameof(ProcessingTimeMs)]));
        }

        return results;
    }
}

/// <summary>
/// Individual sample data item for structured sample data operations
/// </summary>
[DataContract]
[XmlType("SampleDataItem")]
public record SampleDataItem : IValidatableObject
{
    /// <summary>
    /// Unique identifier for the sample item
    /// </summary>
    [DataMember]
    [XmlElement("Id")]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be a positive number")]
    public int Id { get; init; }

    /// <summary>
    /// Sample item name
    /// </summary>
    [DataMember]
    [XmlElement("Name")]
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Sample item description
    /// </summary>
    [DataMember]
    [XmlElement("Description")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; init; }

    /// <summary>
    /// Sample value
    /// </summary>
    [DataMember]
    [XmlElement("Value")]
    public object? Value { get; init; }

    /// <summary>
    /// Data type of the sample value
    /// </summary>
    [DataMember]
    [XmlElement("DataType")]
    [StringLength(50, ErrorMessage = "DataType cannot exceed 50 characters")]
    public string? DataType { get; init; }

    /// <summary>
    /// Sample category or classification
    /// </summary>
    [DataMember]
    [XmlElement("Category")]
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string? Category { get; init; }

    /// <summary>
    /// Sample creation timestamp
    /// </summary>
    [DataMember]
    [XmlElement("CreatedAt")]
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Sample last modified timestamp
    /// </summary>
    [DataMember]
    [XmlElement("LastModified")]
    public DateTime? LastModified { get; init; }

    /// <summary>
    /// Indicates if the sample is active
    /// </summary>
    [DataMember]
    [XmlElement("IsActive")]
    public bool IsActive { get; init; } = true;

    /// <summary>
    /// Sample priority level
    /// </summary>
    [DataMember]
    [XmlElement("Priority")]
    [Range(1, 10, ErrorMessage = "Priority must be between 1 and 10")]
    public int Priority { get; init; } = 5;

    /// <summary>
    /// Tags associated with the sample
    /// </summary>
    [DataMember]
    [XmlArray("Tags")]
    [XmlArrayItem("Tag", Type = typeof(string))]
    public List<string> Tags { get; init; } = [];

    /// <summary>
    /// Additional properties as key-value pairs
    /// </summary>
    [DataMember]
    [XmlArray("Properties")]
    [XmlArrayItem("Property", Type = typeof(KeyValueItem))]
    public List<KeyValueItem> Properties { get; init; } = [];

    /// <summary>
    /// Sample source system
    /// </summary>
    [DataMember]
    [XmlElement("Source")]
    [StringLength(100, ErrorMessage = "Source cannot exceed 100 characters")]
    public string? Source { get; init; }

    /// <summary>
    /// Sample version
    /// </summary>
    [DataMember]
    [XmlElement("Version")]
    [StringLength(20, ErrorMessage = "Version cannot exceed 20 characters")]
    public string? Version { get; init; }

    /// <summary>
    /// Validates the sample data item
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
/// Individual report data item for report operations
/// </summary>
[DataContract]
[XmlType("ReportDataItem")]
public record ReportDataItem : IValidatableObject
{
    /// <summary>
    /// Unique identifier for the report item
    /// </summary>
    [DataMember]
    [XmlElement("Id")]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be a positive number")]
    public int Id { get; init; }

    /// <summary>
    /// Report item name
    /// </summary>
    [DataMember]
    [XmlElement("Name")]
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Report item description
    /// </summary>
    [DataMember]
    [XmlElement("Description")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; init; }

    /// <summary>
    /// Report item value
    /// </summary>
    [DataMember]
    [XmlElement("Value")]
    public object? Value { get; init; }

    /// <summary>
    /// Data type of the report value
    /// </summary>
    [DataMember]
    [XmlElement("DataType")]
    [StringLength(50, ErrorMessage = "DataType cannot exceed 50 characters")]
    public string? DataType { get; init; }

    /// <summary>
    /// Report section this item belongs to
    /// </summary>
    [DataMember]
    [XmlElement("Section")]
    [StringLength(100, ErrorMessage = "Section cannot exceed 100 characters")]
    public string? Section { get; init; }

    /// <summary>
    /// Display order within the section
    /// </summary>
    [DataMember]
    [XmlElement("Order")]
    [Range(0, int.MaxValue, ErrorMessage = "Order must be non-negative")]
    public int Order { get; init; }

    /// <summary>
    /// Report item category
    /// </summary>
    [DataMember]
    [XmlElement("Category")]
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string? Category { get; init; }

    /// <summary>
    /// Unit of measurement for the value
    /// </summary>
    [DataMember]
    [XmlElement("Unit")]
    [StringLength(20, ErrorMessage = "Unit cannot exceed 20 characters")]
    public string? Unit { get; init; }

    /// <summary>
    /// Formatting instructions for display
    /// </summary>
    [DataMember]
    [XmlElement("Format")]
    [StringLength(50, ErrorMessage = "Format cannot exceed 50 characters")]
    public string? Format { get; init; }

    /// <summary>
    /// Indicates if the item is visible in the report
    /// </summary>
    [DataMember]
    [XmlElement("IsVisible")]
    public bool IsVisible { get; init; } = true;

    /// <summary>
    /// Item creation timestamp
    /// </summary>
    [DataMember]
    [XmlElement("CreatedAt")]
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Data source for this item
    /// </summary>
    [DataMember]
    [XmlElement("DataSource")]
    [StringLength(100, ErrorMessage = "DataSource cannot exceed 100 characters")]
    public string? DataSource { get; init; }

    /// <summary>
    /// Additional metadata for the report item
    /// </summary>
    [DataMember]
    [XmlArray("Metadata")]
    [XmlArrayItem("Meta", Type = typeof(KeyValueItem))]
    public List<KeyValueItem> Metadata { get; init; } = [];

    /// <summary>
    /// Calculation method if applicable
    /// </summary>
    [DataMember]
    [XmlElement("CalculationMethod")]
    [StringLength(100, ErrorMessage = "CalculationMethod cannot exceed 100 characters")]
    public string? CalculationMethod { get; init; }

    /// <summary>
    /// Validates the report data item
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

        if (Order < 0)
        {
            results.Add(new ValidationResult("Order must be non-negative", [nameof(Order)]));
        }

        return results;
    }
}

/// <summary>
/// Report metadata containing additional information about the report
/// </summary>
[DataContract]
[XmlType("ReportMetadata")]
public record ReportMetadata
{
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
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; init; }

    /// <summary>
    /// Report parameters used for generation
    /// </summary>
    [DataMember]
    [XmlArray("Parameters")]
    [XmlArrayItem("Parameter", Type = typeof(KeyValueItem))]
    public List<KeyValueItem> Parameters { get; init; } = [];

    /// <summary>
    /// Data date range start
    /// </summary>
    [DataMember]
    [XmlElement("DateRangeStart")]
    public DateTime? DateRangeStart { get; init; }

    /// <summary>
    /// Data date range end
    /// </summary>
    [DataMember]
    [XmlElement("DateRangeEnd")]
    public DateTime? DateRangeEnd { get; init; }

    /// <summary>
    /// Report file size in bytes if applicable
    /// </summary>
    [DataMember]
    [XmlElement("FileSizeBytes")]
    [Range(0, long.MaxValue, ErrorMessage = "FileSizeBytes must be non-negative")]
    public long? FileSizeBytes { get; init; }

    /// <summary>
    /// Source system or service that generated the report
    /// </summary>
    [DataMember]
    [XmlElement("Source")]
    [StringLength(100, ErrorMessage = "Source cannot exceed 100 characters")]
    public string? Source { get; init; }

    /// <summary>
    /// Version of the report format or schema
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
    /// Additional metadata as key-value pairs
    /// </summary>
    [DataMember]
    [XmlArray("AdditionalData")]
    [XmlArrayItem("Data", Type = typeof(KeyValueItem))]
    public List<KeyValueItem> AdditionalData { get; init; } = [];
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
/// Metric item for report metadata
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

    /// <summary>
    /// Metric category
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