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
        /// Validates the report input object with custom business logic.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>A collection of validation results.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
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
            if (Parameters != null)
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
    }
}