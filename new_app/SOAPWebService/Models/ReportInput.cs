using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System;
using System.Globalization;
using System.Linq;

namespace SOAPWebService.Models
{
    /// <summary>
    /// Data transfer object for report input parameters.
    /// Migrated from legacy VB.NET cReportInput class to modern C# with DataContract serialization.
    /// </summary>
    [DataContract(Name = "ReportInput", Namespace = "http://soapwebservice.com/models")]
    [XmlType(TypeName = "ReportInput", Namespace = "http://soapwebservice.com/models")]
    public class ReportInput : IValidatableObject
    {
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
                }
            }

            // Validate asynchronous processing requirements
            if (IsAsync && string.IsNullOrWhiteSpace(NotificationEmail))
            {
                results.Add(new ValidationResult(
                    "Notification email is required for asynchronous report processing.",
                    new[] { nameof(NotificationEmail) }));
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
    }
}