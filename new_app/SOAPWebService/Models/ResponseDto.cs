using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SOAPWebService.Models;

/// <summary>
/// Response data transfer object for SOAP operations
/// </summary>
[DataContract]
[XmlType("Response")]
public class ResponseDto : IValidatableObject
{
    /// <summary>
    /// Collection of data items
    /// </summary>
    [DataMember]
    [XmlElement("Items")]
    public List<DataItem> Items { get; set; } = new();

    /// <summary>
    /// Response message
    /// </summary>
    [DataMember]
    [XmlElement("Message")]
    [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp of the response
    /// </summary>
    [DataMember]
    [XmlElement("Timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Success status of the operation
    /// </summary>
    [DataMember]
    [XmlElement("Success")]
    public bool Success { get; set; } = true;

    /// <summary>
    /// Validates the response object
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        
        if (Items == null)
        {
            results.Add(new ValidationResult("Items collection cannot be null", new[] { nameof(Items) }));
        }
        
        return results;
    }
}

/// <summary>
/// Individual data item
/// </summary>
[DataContract]
[XmlType("DataItem")]
public class DataItem : IValidatableObject
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    [DataMember]
    [XmlElement("Id")]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be a positive number")]
    public int Id { get; set; }

    /// <summary>
    /// Item name
    /// </summary>
    [DataMember]
    [XmlElement("Name")]
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Item description
    /// </summary>
    [DataMember]
    [XmlElement("Description")]
    [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    [DataMember]
    [XmlElement("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Validates the data item
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        
        if (string.IsNullOrWhiteSpace(Name))
        {
            results.Add(new ValidationResult("Name cannot be empty or whitespace", new[] { nameof(Name) }));
        }
        
        return results;
    }
}