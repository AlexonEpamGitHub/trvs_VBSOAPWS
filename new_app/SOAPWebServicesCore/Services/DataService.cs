namespace SOAPWebServicesCore.Services;

public class DataService : IDataService
{
    public string HelloWorld()
    {
        return "Hello World";
    }

    public string GetData(string name)
    {
        return $"Hello {name}, this is a simple SOAP web service response.";
    }

    public DataSet GetDataSet()
    {
        var ds = new DataSet { Name = "SampleDataSet" };
        var dt = new DataTable { Name = "SampleTable" };
        
        // Add columns
        dt.Columns.Add("ID");
        dt.Columns.Add("Name");
        
        // Add rows
        dt.Rows.Add(new List<object> { 1, "Alice" });
        dt.Rows.Add(new List<object> { 2, "Bob" });
        
        ds.Tables.Add(dt);
        return ds;
    }

    public DataSet GetReport(ReportInput reportInput)
    {
        // Just return the same dataset as GetDataSet for this example
        return GetDataSet();
    }
}