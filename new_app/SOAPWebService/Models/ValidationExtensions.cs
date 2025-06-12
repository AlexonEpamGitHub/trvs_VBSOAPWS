using System.ComponentModel.DataAnnotations;

namespace SOAPWebService.Models;

/// <summary>
/// Extension methods for data validation and DTO validation support.
/// Provides utility methods for validating DTOs and handling validation results.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Validates a data transfer object using DataAnnotations.
    /// </summary>
    /// <param name="obj">The object to validate.</param>
    /// <returns>A tuple containing validation success status and list of validation errors.</returns>
    public static (bool IsValid, List<string> Errors) ValidateDto(this object obj)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(obj);
        
        bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);
        
        var errors = validationResults.Select(vr => vr.ErrorMessage ?? "Unknown validation error").ToList();
        
        return (isValid, errors);
    }

    /// <summary>
    /// Validates a DTO and throws ValidationException if invalid.
    /// </summary>
    /// <param name="obj">The object to validate.</param>
    /// <exception cref="ValidationException">Thrown when validation fails.</exception>
    public static void ValidateAndThrow(this object obj)
    {
        var (isValid, errors) = obj.ValidateDto();
        
        if (!isValid)
        {
            var errorMessage = string.Join("; ", errors);
            throw new ValidationException($"Validation failed: {errorMessage}");
        }
    }

    /// <summary>
    /// Creates a validation summary for ResponseDto objects.
    /// </summary>
    /// <param name="obj">The object to validate.</param>
    /// <returns>A string containing validation summary or empty string if valid.</returns>
    public static string GetValidationSummary(this object obj)
    {
        var (isValid, errors) = obj.ValidateDto();
        
        if (isValid)
            return string.Empty;
            
        return $"Validation errors: {string.Join(", ", errors)}";
    }
}

/// <summary>
/// Custom validation attributes for SOAP service DTOs.
/// </summary>
public static class CustomValidation
{
    /// <summary>
    /// Validates that a collection has at least one item.
    /// </summary>
    public class NotEmptyCollectionAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is System.Collections.IEnumerable enumerable)
            {
                var enumerator = enumerable.GetEnumerator();
                return enumerator.MoveNext();
            }
            
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field must contain at least one item.";
        }
    }

    /// <summary>
    /// Validates that a string is not null, empty, or whitespace.
    /// </summary>
    public class NotEmptyStringAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return !string.IsNullOrWhiteSpace(value?.ToString());
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field cannot be empty or whitespace.";
        }
    }

    /// <summary>
    /// Validates that a DateTime is not in the future.
    /// </summary>
    public class NotFutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime <= DateTime.UtcNow;
            }
            
            if (value is DateTime? nullableDateTime)
            {
                return nullableDateTime == null || nullableDateTime.Value <= DateTime.UtcNow;
            }
            
            return true; // Allow null values
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field cannot be in the future.";
        }
    }
}