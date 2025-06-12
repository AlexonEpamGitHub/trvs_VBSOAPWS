using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using SOAPWebService.Models;
using Xunit;

namespace SOAPWebService.Tests;

/// <summary>
/// Tests to ensure proper XML and DataContract serialization for SOAP compatibility.
/// Verifies that the DTOs can replace legacy DataSet serialization behavior.
/// </summary>
public class SerializationTests
{
    [Fact]
    public void SampleDataResponse_XmlSerialization_WorksCorrectly()
    {
        // Arrange
        var response = ResponseDtoFactory.CreateSampleDataResponse();
        var serializer = new XmlSerializer(typeof(SampleDataResponse));

        // Act
        string xml;
        using (var stringWriter = new StringWriter())
        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
        {
            serializer.Serialize(xmlWriter, response);
            xml = stringWriter.ToString();
        }

        // Assert
        Assert.Contains("SampleDataResponse", xml);
        Assert.Contains("DataSetName", xml);
        Assert.Contains("SampleDataSet", xml);
        Assert.Contains("Alice", xml);
        Assert.Contains("Bob", xml);
        Assert.Contains("<ID>1</ID>", xml);
        Assert.Contains("<ID>2</ID>", xml);
    }

    [Fact]
    public void SampleDataResponse_XmlDeserialization_WorksCorrectly()
    {
        // Arrange
        var originalResponse = ResponseDtoFactory.CreateSampleDataResponse();
        var serializer = new XmlSerializer(typeof(SampleDataResponse));
        
        string xml;
        using (var stringWriter = new StringWriter())
        using (var xmlWriter = XmlWriter.Create(stringWriter))
        {
            serializer.Serialize(xmlWriter, originalResponse);
            xml = stringWriter.ToString();
        }

        // Act
        SampleDataResponse? deserializedResponse;
        using (var stringReader = new StringReader(xml))
        using (var xmlReader = XmlReader.Create(stringReader))
        {
            deserializedResponse = (SampleDataResponse?)serializer.Deserialize(xmlReader);
        }

        // Assert
        Assert.NotNull(deserializedResponse);
        Assert.Equal(originalResponse.Status, deserializedResponse.Status);
        Assert.Equal(originalResponse.DataSetName, deserializedResponse.DataSetName);
        Assert.Equal(originalResponse.Count, deserializedResponse.Count);
        Assert.Equal(originalResponse.SampleData.Count, deserializedResponse.SampleData.Count);
        
        for (int i = 0; i < originalResponse.SampleData.Count; i++)
        {
            Assert.Equal(originalResponse.SampleData[i].ID, deserializedResponse.SampleData[i].ID);
            Assert.Equal(originalResponse.SampleData[i].Name, deserializedResponse.SampleData[i].Name);
        }
    }

    [Fact]
    public void ReportDataResponse_XmlSerialization_WorksCorrectly()
    {
        // Arrange
        var response = ResponseDtoFactory.CreateReportDataResponse("Test Report");
        var serializer = new XmlSerializer(typeof(ReportDataResponse));

        // Act
        string xml;
        using (var stringWriter = new StringWriter())
        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
        {
            serializer.Serialize(xmlWriter, response);
            xml = stringWriter.ToString();
        }

        // Assert
        Assert.Contains("ReportDataResponse", xml);
        Assert.Contains("ReportName", xml);
        Assert.Contains("Test Report", xml);
        Assert.Contains("ReportDataSet", xml);
        Assert.Contains("ReportMetadata", xml);
        Assert.Contains("Category", xml);
        Assert.Contains("CreatedDate", xml);
    }

    [Fact]
    public void ReportDataResponse_DataContractSerialization_WorksCorrectly()
    {
        // Arrange
        var response = ResponseDtoFactory.CreateReportDataResponse("Contract Test");
        var serializer = new DataContractSerializer(typeof(ReportDataResponse));

        // Act
        string xml;
        using (var memoryStream = new MemoryStream())
        {
            serializer.WriteObject(memoryStream, response);
            memoryStream.Position = 0;
            using (var reader = new StreamReader(memoryStream))
            {
                xml = reader.ReadToEnd();
            }
        }

        // Assert
        Assert.Contains("ReportDataResponse", xml);
        Assert.Contains("Contract Test", xml);
        Assert.Contains("ReportData", xml);
    }

    [Fact]
    public void ReportInput_XmlSerialization_WorksCorrectly()
    {
        // Arrange
        var input = new ReportInput { ReportName = "Serialization Test" };
        var serializer = new XmlSerializer(typeof(ReportInput));

        // Act
        string xml;
        using (var stringWriter = new StringWriter())
        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
        {
            serializer.Serialize(xmlWriter, input);
            xml = stringWriter.ToString();
        }

        // Assert
        Assert.Contains("ReportInput", xml);
        Assert.Contains("ReportName", xml);
        Assert.Contains("Serialization Test", xml);
    }

    [Fact]
    public void ReportInput_XmlDeserialization_WorksCorrectly()
    {
        // Arrange
        var originalInput = new ReportInput { ReportName = "Deserialization Test" };
        var serializer = new XmlSerializer(typeof(ReportInput));
        
        string xml;
        using (var stringWriter = new StringWriter())
        using (var xmlWriter = XmlWriter.Create(stringWriter))
        {
            serializer.Serialize(xmlWriter, originalInput);
            xml = stringWriter.ToString();
        }

        // Act
        ReportInput? deserializedInput;
        using (var stringReader = new StringReader(xml))
        using (var xmlReader = XmlReader.Create(stringReader))
        {
            deserializedInput = (ReportInput?)serializer.Deserialize(xmlReader);
        }

        // Assert
        Assert.NotNull(deserializedInput);
        Assert.Equal(originalInput.ReportName, deserializedInput.ReportName);
    }

    [Fact]
    public void SampleDataItem_XmlSerialization_HandlesEmptyName()
    {
        // Arrange
        var item = new SampleDataItem { ID = 1, Name = "" };
        var serializer = new XmlSerializer(typeof(SampleDataItem));

        // Act
        string xml;
        using (var stringWriter = new StringWriter())
        using (var xmlWriter = XmlWriter.Create(stringWriter))
        {
            serializer.Serialize(xmlWriter, item);
            xml = stringWriter.ToString();
        }

        // Assert
        Assert.Contains("SampleDataItem", xml);
        Assert.Contains("<ID>1</ID>", xml);
        Assert.Contains("<Name", xml); // Name element should be present even if empty
    }

    [Fact]
    public void ErrorResponse_XmlSerialization_PreservesErrorInformation()
    {
        // Arrange
        var errorResponse = ResponseDtoFactory.CreateErrorSampleDataResponse("Test error message");
        var serializer = new XmlSerializer(typeof(SampleDataResponse));

        // Act
        string xml;
        using (var stringWriter = new StringWriter())
        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
        {
            serializer.Serialize(xmlWriter, errorResponse);
            xml = stringWriter.ToString();
        }

        // Assert
        Assert.Contains("SampleDataResponse", xml);
        Assert.Contains("Error", xml);
        Assert.Contains("Test error message", xml);
        Assert.Contains("<Count>0</Count>", xml);
    }

    [Fact]
    public void LargeDataResponse_XmlSerialization_HandlesMultipleItems()
    {
        // Arrange
        var response = new SampleDataResponse
        {
            Status = "Success",
            DataSetName = "LargeDataSet",
            TableName = "LargeTable",
            SampleData = Enumerable.Range(1, 100)
                .Select(i => new SampleDataItem { ID = i, Name = $"User{i}" })
                .ToList()
        };
        var serializer = new XmlSerializer(typeof(SampleDataResponse));

        // Act
        string xml;
        using (var stringWriter = new StringWriter())
        using (var xmlWriter = XmlWriter.Create(stringWriter))
        {
            serializer.Serialize(xmlWriter, response);
            xml = stringWriter.ToString();
        }

        // Assert
        Assert.Contains("SampleDataResponse", xml);
        Assert.Contains("User1", xml);
        Assert.Contains("User100", xml);
        Assert.Contains("<Count>100</Count>", xml);
    }

    [Fact]
    public void NullReportName_Serialization_HandlesNullGracefully()
    {
        // Arrange
        var response = ResponseDtoFactory.CreateReportDataResponse(null);
        var serializer = new XmlSerializer(typeof(ReportDataResponse));

        // Act & Assert - Should not throw exception
        string xml;
        using (var stringWriter = new StringWriter())
        using (var xmlWriter = XmlWriter.Create(stringWriter))
        {
            serializer.Serialize(xmlWriter, response);
            xml = stringWriter.ToString();
        }

        Assert.Contains("ReportDataResponse", xml);
    }
}