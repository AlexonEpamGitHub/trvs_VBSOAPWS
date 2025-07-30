namespace SOAPWebServicesSimple.Models;

/// <summary>
/// Represents input data for report processing
/// </summary>
public class ReportInput
{
    private string _reportName = string.Empty;
    private DateTime _processedTimestamp;

    /// <summary>
    /// Gets or sets the name of the report
    /// </summary>
    public string ReportName
    {
        get => _reportName;
        set => _reportName = value;
    }

    /// <summary>
    /// Gets or sets the timestamp when the report was processed
    /// </summary>
    public DateTime ProcessedTimestamp
    {
        get => _processedTimestamp;
        set => _processedTimestamp = value;
    }
}
