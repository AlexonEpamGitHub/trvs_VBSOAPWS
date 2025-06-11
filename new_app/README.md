# SOAP Web Services Migration to .NET 8

This project is a migration of the legacy VB.NET SOAP Web Service to a modern .NET 8 C# implementation.

## Migration Details

The original legacy application was a Visual Basic Web Service (.asmx) running on .NET Framework 4.5.2. The new application uses .NET 8 with the SoapCore library to provide SOAP web service functionality.

### Key Components

- **SOAPWebServicesCore** - The main project containing the SOAP service implementation
  - Uses SoapCore for SOAP protocol support
  - Implements the same methods as the legacy service
  - Configures SOAP endpoints with the same URLs for compatibility

### Endpoints

- `/GetDataService.asmx` - SOAP service with the following methods:
  - `HelloWorld()` - Returns a simple greeting
  - `GetData(string name)` - Returns a personalized greeting
  - `GetDataSet()` - Returns a sample DataSet
  - `GetReport(ref ReportInput reportInput)` - Returns a sample DataSet based on the report input

## Building and Running

1. Open the solution in Visual Studio 2022
2. Build the solution
3. Run the project
4. Access the SOAP service at http://localhost:57114/GetDataService.asmx

## Technologies Used

- .NET 8
- C# 12
- SoapCore 1.1.0.42
- System.ServiceModel packages