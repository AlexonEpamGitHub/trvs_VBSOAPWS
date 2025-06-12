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
    [XmlElement("Items")]
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
    /// Additional properties as key-value pairs
    /// </summary>
    [DataMember]
    [XmlElement("Properties")]
    public Dictionary<string, object?> Properties { get; init; } = [];

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
        
        return results;
    }
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
    /// Additional metadata as key-value pairs
    /// </summary>
    [DataMember]
    [XmlElement("AdditionalData")]
    public Dictionary<string, object?> AdditionalData { get; init; } = [];
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
    /// Report parameters used for generation
    /// </summary>
    [DataMember]
    [XmlElement("Parameters")]
    public Dictionary<string, object?> Parameters { get; init; } = [];

    /// <summary>
    /// Collection of report sections or categories
    /// </summary>
    [DataMember]
    [XmlElement("Sections")]
    public List<ReportSection> Sections { get; init; } = [];

    /// <summary>
    /// Report summary information
    /// </summary>
    [DataMember]
    [XmlElement("Summary")]
    public ReportSummary? Summary { get; init; }
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
    /// Section order for display
    /// </summary>
    [DataMember]
    [XmlElement("Order")]
    [Range(0, int.MaxValue, ErrorMessage = "Order must be non-negative")]
    public int Order { get; init; }

    /// <summary>
    /// Data items in this section
    /// </summary>
    [DataMember]
    [XmlElement("Data")]
    public List<DataItem> Data { get; init; } = [];

    /// <summary>
    /// Section-specific metadata
    /// </summary>
    [DataMember]
    [XmlElement("Metadata")]
    public Dictionary<string, object?> Metadata { get; init; } = [];
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
    /// Collection of warnings encountered during processing
    /// </summary>
    [DataMember]
    [XmlElement("Warnings")]
    public List<string> Warnings { get; init; } = [];

    /// <summary>
    /// Collection of errors encountered during processing
    /// </summary>
    [DataMember]
    [XmlElement("Errors")]
    public List<string> Errors { get; init; } = [];

    /// <summary>
    /// Key performance indicators or metrics
    /// </summary>
    [DataMember]
    [XmlElement("Metrics")]
    public Dictionary<string, decimal> Metrics { get; init; } = [];
}