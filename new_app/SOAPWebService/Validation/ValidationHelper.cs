using System.ComponentModel.DataAnnotations;

namespace SOAPWebService.Validation;

/// <summary>
/// Provides helper methods for validation operations in the SOAP web service.
/// Supports .NET 8 validation patterns and nullable reference types.
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Validates an object and returns all validation results.
    /// </summary>
    /// <param name="instance">The object instance to validate.</param>
    /// <param name="serviceProvider">Optional service provider for dependency injection.</param>
    /// <returns>A collection of validation results. Empty if validation passes.</returns>
    public static IEnumerable<ValidationResult> ValidateObject(object instance, IServiceProvider? serviceProvider = null)
    {
        ArgumentNullException.ThrowIfNull(instance);

        var results = new List<ValidationResult>();
        var context = new ValidationContext(instance, serviceProvider, null);

        // Perform data annotation validation
        Validator.TryValidateObject(instance, context, results, validateAllProperties: true);

        // If the object implements IValidatableObject, perform custom validation
        if (instance is IValidatableObject validatableObject)
        {
            var customResults = validatableObject.Validate(context);
            results.AddRange(customResults);
        }

        return results;
    }

    /// <summary>
    /// Validates an object and throws a ValidationException if validation fails.
    /// </summary>
    /// <param name="instance">The object instance to validate.</param>
    /// <param name="serviceProvider">Optional service provider for dependency injection.</param>
    /// <exception cref="ValidationException">Thrown when validation fails.</exception>
    public static void ValidateAndThrow(object instance, IServiceProvider? serviceProvider = null)
    {
        var results = ValidateObject(instance, serviceProvider);
        var validationResults = results.ToList();

        if (validationResults.Count != 0)
        {
            var errorMessages = validationResults.Select(r => r.ErrorMessage ?? "Unknown validation error");
            var combinedMessage = string.Join("; ", errorMessages);
            throw new ValidationException($"Validation failed: {combinedMessage}");
        }
    }

    /// <summary>
    /// Checks if an object is valid without throwing exceptions.
    /// </summary>
    /// <param name="instance">The object instance to validate.</param>
    /// <param name="serviceProvider">Optional service provider for dependency injection.</param>
    /// <returns>True if the object is valid, false otherwise.</returns>
    public static bool IsValid(object instance, IServiceProvider? serviceProvider = null)
    {
        try
        {
            var results = ValidateObject(instance, serviceProvider);
            return !results.Any();
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates a specific property of an object.
    /// </summary>
    /// <param name="instance">The object instance containing the property.</param>
    /// <param name="propertyName">The name of the property to validate.</param>
    /// <param name="propertyValue">The value of the property to validate.</param>
    /// <param name="serviceProvider">Optional service provider for dependency injection.</param>
    /// <returns>A collection of validation results for the property.</returns>
    public static IEnumerable<ValidationResult> ValidateProperty(object instance, string propertyName, object? propertyValue, IServiceProvider? serviceProvider = null)
    {
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        var results = new List<ValidationResult>();
        var context = new ValidationContext(instance, serviceProvider, null)
        {
            MemberName = propertyName
        };

        Validator.TryValidateProperty(propertyValue, context, results);
        return results;
    }

    /// <summary>
    /// Sanitizes a string value by trimming whitespace and handling null values.
    /// </summary>
    /// <param name="value">The string value to sanitize.</param>
    /// <param name="defaultValue">The default value to use if the input is null or empty.</param>
    /// <returns>The sanitized string value.</returns>
    public static string SanitizeString(string? value, string defaultValue = "")
    {
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
    }

    /// <summary>
    /// Checks if a string contains any prohibited characters for file names or report names.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <returns>True if the string contains prohibited characters, false otherwise.</returns>
    public static bool ContainsProhibitedCharacters(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var prohibitedChars = new[] { '<', '>', '|', '*', '?', '"', ':', '\\', '/' };
        return value.IndexOfAny(prohibitedChars) >= 0;
    }

    /// <summary>
    /// Checks if a string is a reserved system name that should not be allowed.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <returns>True if the string is a reserved name, false otherwise.</returns>
    public static bool IsReservedName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var reservedNames = new[]
        {
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        };

        return reservedNames.Contains(value.ToUpperInvariant());
    }

    /// <summary>
    /// Creates a standardized error message for validation failures.
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation.</param>
    /// <param name="errorType">The type of validation error.</param>
    /// <param name="additionalInfo">Additional information about the error.</param>
    /// <returns>A formatted error message string.</returns>
    public static string CreateErrorMessage(string propertyName, string errorType, string? additionalInfo = null)
    {
        var baseMessage = $"{propertyName} validation failed: {errorType}";
        return string.IsNullOrWhiteSpace(additionalInfo) ? baseMessage : $"{baseMessage}. {additionalInfo}";
    }
}