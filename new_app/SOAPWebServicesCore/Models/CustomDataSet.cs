using System.Runtime.Serialization;

namespace SOAPWebServicesCore.Models;

[DataContract]
public class CustomDataSet
{
    [DataMember]
    public string Name { get; set; }
    
    [DataMember]
    public List<CustomDataTable> Tables { get; set; } = new List<CustomDataTable>();
}

[DataContract]
public class CustomDataTable
{
    [DataMember]
    public string Name { get; set; }
    
    [DataMember]
    public List<string> Columns { get; set; } = new List<string>();
    
    [DataMember]
    public List<List<object>> Rows { get; set; } = new List<List<object>>();
}