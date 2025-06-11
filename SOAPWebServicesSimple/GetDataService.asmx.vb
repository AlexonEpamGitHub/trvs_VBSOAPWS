Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class GetDataService
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function

    <WebMethod()>
    Public Function GetData(Name As String) As String
        Return "Hello " + Name + ", this is a simple SOAP web service response."
    End Function

    <WebMethod()>
    Public Function GetDataSet() As DataSet
        Dim ds As New DataSet("SampleDataSet")
        Dim dt As New DataTable("SampleTable")
        dt.Columns.Add("ID", GetType(Integer))
        dt.Columns.Add("Name", GetType(String))
        ' Adding some sample data
        dt.Rows.Add(1, "Alice")
        dt.Rows.Add(2, "Bob")
        ds.Tables.Add(dt)
        Return ds
    End Function

    <WebMethod()>
    Public Function GetReport(ByRef reporInput As cReportInput) As DataSet

        Return GetDataSet()
    End Function



End Class