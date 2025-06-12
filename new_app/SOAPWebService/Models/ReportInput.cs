using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SOAPWebService.Models;

/// <summary>
/// Data transfer object for report input parameters.
/// Migrated from legacy VB.NET cReportInput class to modern C# with DataContract serialization.
/// </summary>
[DataContract(Name = "ReportInput", Namespace = "http://soapwebservice.com/models")]
[XmlType(TypeName = "ReportInput", Namespace = "http://soapwebservice.com/models")]
[Serializable]
public class ReportInput : IValidatableObject, IEquatable<ReportInput>
{
    /// <summary>
    /// List of reserved report names that cannot be used.
    /// </summary>
    private static readonly HashSet<string> ReservedNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9", "SYSTEM", "ADMIN", "ROOT"
    };

    /// <summary>
    /// Characters that are prohibited in report names and parameters.
    /// </summary>
    private static readonly char[] ProhibitedCharacters = ['<', '>', '"', '|', '?', '*', '/', '\\', ':', ';'];

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportInput"/> class.
    /// </summary>
    public ReportInput()
    {
        ReportName = string.Empty;
        Priority = 5;
        IsAsync = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportInput"/> class with the specified report name.
    /// </summary>
    /// <param name="reportName">The name of the report to generate.</param>
    /// <exception cref="ArgumentException">Thrown when reportName is null or whitespace.</exception>
    public ReportInput(string reportName) : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reportName);
        ReportName = reportName;
    }

    /// <summary>
    /// Gets or sets the name of the report to generate.
    /// </summary>
    [DataMember(Name = "ReportName", Order = 1, IsRequired = true)]
    [XmlElement("ReportName")]
    [Required(ErrorMessage = "Report name is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Report name must be between 1 and 100 characters.")]
    public string ReportName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parameters for the report generation.
    /// </summary>
    [DataMember(Name = "Parameters", Order = 2, IsRequired = false)]
    [XmlElement("Parameters")]
    public Dictionary<string, object>? Parameters { get; set; }

    /// <summary>
    /// Gets or sets the start date for the report data range.
    /// </summary>
    [DataMember(Name = "StartDate", Order = 3, IsRequired = false)]
    [XmlElement("StartDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date for the report data range.
    /// </summary>
    [DataMember(Name = "EndDate", Order = 4, IsRequired = false)]
    [XmlElement("EndDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the output format for the report.
    /// </summary>
    [DataMember(Name = "Format", Order = 5, IsRequired = false)]
    [XmlElement("Format")]
    [StringLength(20, ErrorMessage = "Format must not exceed 20 characters.")]
    public string? Format { get; set; }

    /// <summary>
    /// Gets or sets the user ID requesting the report.
    /// </summary>
    [DataMember(Name = "UserId", Order = 6, IsRequired = false)]
    [XmlElement("UserId")]
    [StringLength(50, ErrorMessage = "User ID must not exceed 50 characters.")]
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the culture information for report localization.
    /// </summary>
    [DataMember(Name = "Culture", Order = 7, IsRequired = false)]
    [XmlElement("Culture")]
    [StringLength(10, ErrorMessage = "Culture must not exceed 10 characters.")]
    public string? Culture { get; set; }

    /// <summary>
    /// Gets or sets the priority level for report processing.
    /// </summary>
    [DataMember(Name = "Priority", Order = 8, IsRequired = false)]
    [XmlElement("Priority")]
    [Range(1, 10, ErrorMessage = "Priority must be between 1 and 10.")]
    public int Priority { get; set; } = 5;

    /// <summary>
    /// Gets or sets whether the report should be generated asynchronously.
    /// </summary>
    [DataMember(Name = "IsAsync", Order = 9, IsRequired = false)]
    [XmlElement("IsAsync")]
    public bool IsAsync { get; set; } = false;

    /// <summary>
    /// Gets or sets the email address for report delivery notification.
    /// </summary>
    [DataMember(Name = "NotificationEmail", Order = 10, IsRequired = false)]
    [XmlElement("NotificationEmail")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(255, ErrorMessage = "Email address must not exceed 255 characters.")]
    public string? NotificationEmail { get; set; }

    /// <summary>
    /// Gets or sets the template ID to use for report generation.
    /// </summary>
    [DataMember(Name = "TemplateId", Order = 11, IsRequired = false)]
    [XmlElement("TemplateId")]
    [StringLength(50, ErrorMessage = "Template ID must not exceed 50 characters.")]
    public string? TemplateId { get; set; }

    /// <summary>
    /// Gets or sets additional metadata for the report.
    /// </summary>
    [DataMember(Name = "Metadata", Order = 12, IsRequired = false)]
    [XmlElement("Metadata")]
    public Dictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// Validates the report input object with custom business logic.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        ArgumentNullException.ThrowIfNull(validationContext);
        
        var results = new List<ValidationResult>();

        // Validate date range
        if (StartDate.HasValue && EndDate.HasValue)
        {
            if (StartDate.Value > EndDate.Value)
            {
                results.Add(new ValidationResult(
                    "Start date cannot be greater than end date.",
                    new[] { nameof(StartDate), nameof(EndDate) }));
            }

            // Check if date range is not too far in the future
            var maxFutureDate = DateTime.Now.AddYears(1);
            if (StartDate.Value > maxFutureDate || EndDate.Value > maxFutureDate)
            {
                results.Add(new ValidationResult(
                    "Date range cannot exceed one year from current date.",
                    new[] { nameof(StartDate), nameof(EndDate) }));
            }

            // Check if date range is not too long
            var dateSpan = EndDate.Value - StartDate.Value;
            if (dateSpan.TotalDays > 365)
            {
                results.Add(new ValidationResult(
                    "Date range cannot exceed 365 days.",
                    new[] { nameof(StartDate), nameof(EndDate) }));
            }
        }

        // Validate format
        if (!string.IsNullOrWhiteSpace(Format))
        {
            var validFormats = new[] { "PDF", "EXCEL", "CSV", "XML", "JSON" };
            if (!validFormats.Contains(Format.ToUpperInvariant()))
            {
                results.Add(new ValidationResult(
                    "Format must be one of: PDF, EXCEL, CSV, XML, JSON.",
                    new[] { nameof(Format) }));
            }
        }

        // Validate report name format
        if (!string.IsNullOrWhiteSpace(ReportName))
        {
            if (!IsValidReportNameFormat(ReportName))
            {
                results.Add(new ValidationResult(
                    "Report name contains invalid characters. Only alphanumeric characters, spaces, hyphens, and underscores are allowed.",
                    new[] { nameof(ReportName) }));
            }

            if (ContainsProhibitedCharacters(ReportName))
            {
                results.Add(new ValidationResult(
                    "Report name contains prohibited characters.",
                    new[] { nameof(ReportName) }));
            }

            if (IsReservedName(ReportName))
            {
                results.Add(new ValidationResult(
                    "Report name is a reserved system name and cannot be used.",
                    new[] { nameof(ReportName) }));
            }
        }

        // Validate parameters
        if (Parameters is not null)
        {
            if (Parameters.Count > 50)
            {
                results.Add(new ValidationResult(
                    "Maximum of 50 parameters allowed.",
                    new[] { nameof(Parameters) }));
            }

            foreach (var parameter in Parameters)
            {
                if (string.IsNullOrWhiteSpace(parameter.Key))
                {
                    results.Add(new ValidationResult(
                        "Parameter keys cannot be null or empty.",
                        new[] { nameof(Parameters) }));
                    break;
                }

                if (parameter.Key.Length > 100)
                {
                    results.Add(new ValidationResult(
                        $"Parameter key '{parameter.Key}' exceeds maximum length of 100 characters.",
                        new[] { nameof(Parameters) }));
                }

                if (ContainsProhibitedCharacters(parameter.Key))
                {
                    results.Add(new ValidationResult(
                        $"Parameter key '{parameter.Key}' contains prohibited characters.",
                        new[] { nameof(Parameters) }));
                }
            }
        }

        // Validate culture information
        if (!string.IsNullOrWhiteSpace(Culture))
        {
            if (!IsValidCultureInfo(Culture))
            {
                results.Add(new ValidationResult(
                    "Invalid culture information provided.",
                    new[] { nameof(Culture) }));
            }
        }

        // Validate metadata
        if (Metadata is not null)
        {
            if (Metadata.Count > 20)
            {
                results.Add(new ValidationResult(
                    "Maximum of 20 metadata entries allowed.",
                    new[] { nameof(Metadata) }));
            }

            foreach (var metadata in Metadata)
            {
                if (string.IsNullOrWhiteSpace(metadata.Key))
                {
                    results.Add(new ValidationResult(
                        "Metadata keys cannot be null or empty.",
                        new[] { nameof(Metadata) }));
                    break;
                }

                if (metadata.Key.Length > 50)
                {
                    results.Add(new ValidationResult(
                        $"Metadata key '{metadata.Key}' exceeds maximum length of 50 characters.",
                        new[] { nameof(Metadata) }));
                }

                if (!string.IsNullOrEmpty(metadata.Value) && metadata.Value.Length > 255)
                {
                    results.Add(new ValidationResult(
                        $"Metadata value for key '{metadata.Key}' exceeds maximum length of 255 characters.",
                        new[] { nameof(Metadata) }));
                }

                if (ContainsProhibitedCharacters(metadata.Key))
                {
                    results.Add(new ValidationResult(
                        $"Metadata key '{metadata.Key}' contains prohibited characters.",
                        new[] { nameof(Metadata) }));
                }
            }
        }

        // Validate asynchronous processing requirements
        if (IsAsync && string.IsNullOrWhiteSpace(NotificationEmail))
        {
            results.Add(new ValidationResult(
                "Notification email is required for asynchronous report processing.",
                new[] { nameof(NotificationEmail) }));
        }

        // Validate user ID format
        if (!string.IsNullOrWhiteSpace(UserId) && ContainsProhibitedCharacters(UserId))
        {
            results.Add(new ValidationResult(
                "User ID contains prohibited characters.",
                new[] { nameof(UserId) }));
        }

        // Validate template ID format
        if (!string.IsNullOrWhiteSpace(TemplateId) && ContainsProhibitedCharacters(TemplateId))
        {
            results.Add(new ValidationResult(
                "Template ID contains prohibited characters.",
                new[] { nameof(TemplateId) }));
        }

        return results;
    }

    /// <summary>
    /// Validates if the report name format is acceptable.
    /// </summary>
    /// <param name="reportName">The report name to validate.</param>
    /// <returns>True if the format is valid, otherwise false.</returns>
    private static bool IsValidReportNameFormat(string reportName)
    {
        if (string.IsNullOrWhiteSpace(reportName))
            return false;

        // Allow alphanumeric characters, spaces, hyphens, and underscores
        return reportName.All(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '_');
    }

    /// <summary>
    /// Validates if the culture information is valid.
    /// </summary>
    /// <param name="culture">The culture string to validate.</param>
    /// <returns>True if the culture is valid, otherwise false.</returns>
    private static bool IsValidCultureInfo(string culture)
    {
        if (string.IsNullOrWhiteSpace(culture))
            return false;

        try
        {
            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            return cultureInfo is not null;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if the specified text contains any prohibited characters.
    /// </summary>
    /// <param name="text">The text to check.</param>
    /// <returns>True if prohibited characters are found, otherwise false.</returns>
    private static bool ContainsProhibitedCharacters(string text)
    {
        return !string.IsNullOrEmpty(text) && text.IndexOfAny(ProhibitedCharacters) >= 0;
    }

    /// <summary>
    /// Checks if the specified name is a reserved system name.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>True if the name is reserved, otherwise false.</returns>
    private static bool IsReservedName(string name)
    {
        return !string.IsNullOrWhiteSpace(name) && ReservedNames.Contains(name.Trim());
    }

    /// <summary>
    /// Returns a string representation of the ReportInput object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"ReportName: {ReportName}");
        sb.AppendLine($"Format: {Format ?? "Not specified"}");
        sb.AppendLine($"UserId: {UserId ?? "Not specified"}");
        sb.AppendLine($"Priority: {Priority}");
        sb.AppendLine($"IsAsync: {IsAsync}");
        
        if (StartDate.HasValue)
            sb.AppendLine($"StartDate: {StartDate.Value:yyyy-MM-dd}");
        
        if (EndDate.HasValue)
            sb.AppendLine($"EndDate: {EndDate.Value:yyyy-MM-dd}");
        
        if (Parameters?.Count > 0)
            sb.AppendLine($"Parameters: {Parameters.Count} item(s)");
        
        if (Metadata?.Count > 0)
            sb.AppendLine($"Metadata: {Metadata.Count} item(s)");

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as ReportInput);
    }

    /// <summary>
    /// Determines whether the specified ReportInput is equal to the current ReportInput.
    /// </summary>
    /// <param name="other">The ReportInput to compare with the current ReportInput.</param>
    /// <returns>True if the specified ReportInput is equal to the current ReportInput; otherwise, false.</returns>
    public bool Equals(ReportInput? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return string.Equals(ReportName, other.ReportName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Format, other.Format, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(UserId, other.UserId, StringComparison.Ordinal) &&
               string.Equals(Culture, other.Culture, StringComparison.Ordinal) &&
               string.Equals(NotificationEmail, other.NotificationEmail, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(TemplateId, other.TemplateId, StringComparison.Ordinal) &&
               Priority == other.Priority &&
               IsAsync == other.IsAsync &&
               Nullable.Equals(StartDate, other.StartDate) &&
               Nullable.Equals(EndDate, other.EndDate) &&
               DictionariesEqual(Parameters, other.Parameters) &&
               DictionariesEqual(Metadata, other.Metadata);
    }

    /// <summary>
    /// Returns a hash code for the current object.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(ReportName, StringComparer.OrdinalIgnoreCase);
        hash.Add(Format, StringComparer.OrdinalIgnoreCase);
        hash.Add(UserId, StringComparer.Ordinal);
        hash.Add(Culture, StringComparer.Ordinal);
        hash.Add(NotificationEmail, StringComparer.OrdinalIgnoreCase);
        hash.Add(TemplateId, StringComparer.Ordinal);
        hash.Add(Priority);
        hash.Add(IsAsync);
        hash.Add(StartDate);
        hash.Add(EndDate);
        
        if (Parameters is not null)
        {
            foreach (var kvp in Parameters.OrderBy(p => p.Key))
            {
                hash.Add(kvp.Key);
                hash.Add(kvp.Value);
            }
        }
        
        if (Metadata is not null)
        {
            foreach (var kvp in Metadata.OrderBy(m => m.Key))
            {
                hash.Add(kvp.Key);
                hash.Add(kvp.Value);
            }
        }

        return hash.ToHashCode();
    }

    /// <summary>
    /// Compares two dictionaries for equality.
    /// </summary>
    /// <typeparam name="TKey">The type of the dictionary keys.</typeparam>
    /// <typeparam name="TValue">The type of the dictionary values.</typeparam>
    /// <param name="dict1">The first dictionary to compare.</param>
    /// <param name="dict2">The second dictionary to compare.</param>
    /// <returns>True if the dictionaries are equal, otherwise false.</returns>
    private static bool DictionariesEqual<TKey, TValue>(Dictionary<TKey, TValue>? dict1, Dictionary<TKey, TValue>? dict2)
        where TKey : notnull
    {
        if (dict1 is null && dict2 is null) return true;
        if (dict1 is null || dict2 is null) return false;
        if (dict1.Count != dict2.Count) return false;

        foreach (var kvp in dict1)
        {
            if (!dict2.TryGetValue(kvp.Key, out var value) || !Equals(kvp.Value, value))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Equality operator for ReportInput objects.
    /// </summary>
    /// <param name="left">The left ReportInput to compare.</param>
    /// <param name="right">The right ReportInput to compare.</param>
    /// <returns>True if the objects are equal, otherwise false.</returns>
    public static bool operator ==(ReportInput? left, ReportInput? right)
    {
        return EqualityComparer<ReportInput>.Default.Equals(left, right);
    }

    /// <summary>
    /// Inequality operator for ReportInput objects.
    /// </summary>
    /// <param name="left">The left ReportInput to compare.</param>
    /// <param name="right">The right ReportInput to compare.</param>
    /// <returns>True if the objects are not equal, otherwise false.</returns>
    public static bool operator !=(ReportInput? left, ReportInput? right)
    {
        return !(left == right);
    }
}