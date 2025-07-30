namespace SOAPWebServicesSimple.Models;

/// <summary>
/// Represents input data for report processing
/// </summary>
public class ReportInput
{
    private string _reportName = string.Empty;
    private DateTime _processedTimestamp;
    private int _reportId;
    private string _reportType = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the report
    /// </summary>
    public int ReportId
    {
        get => _reportId;
        set => _reportId = value;
    }

    /// <summary>
    /// Gets or sets the name of the report
    /// </summary>
    public string ReportName
    {
        get => _reportName;
        set => _reportName = value;
    }

    /// <summary>
    /// Gets or sets the type of the report
    /// </summary>
    public string ReportType
    {
        get => _reportType;
        set => _reportType = value;
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
