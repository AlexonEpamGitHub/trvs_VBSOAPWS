using System.ServiceModel;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace SOAPWebServicesCore.Services;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface IDataService
{
    [OperationContract]
    string HelloWorld();

    [OperationContract]
    string GetData(string name);

    [OperationContract]
    DataSet GetDataSet();
    
    [OperationContract]
    DataSet GetReport(ReportInput reportInput);
}

// Custom DataSet class for .NET Core (since System.Data.DataSet is not available in .NET Core in the same way)
[DataContract]
public class DataSet
{
    [DataMember]
    public string Name { get; set; }
    
    [DataMember]
    public List<DataTable> Tables { get; set; } = new List<DataTable>();
}

[DataContract]
public class DataTable
{
    [DataMember]
    public string Name { get; set; }
    
    [DataMember]
    public List<string> Columns { get; set; } = new List<string>();
    
    [DataMember]
    public List<List<object>> Rows { get; set; } = new List<List<object>>();
}

[DataContract]
public class ReportInput
{
    [DataMember]
    public string ReportName { get; set; }
}