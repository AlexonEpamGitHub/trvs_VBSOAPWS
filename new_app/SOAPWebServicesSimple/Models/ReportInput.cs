namespace SOAPWebServicesSimple.Models;

public class ReportInput
{
    private string _reportName = string.Empty;

    public string ReportName
    {
        get => _reportName;
        set => _reportName = value;
    }
}