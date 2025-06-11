# SOAP Web Service Migration

This project is a migration of the legacy Visual Basic SOAP Web Service to a modern .NET 8 C# implementation using CoreWCF.

## Migration Details

- Migrated from: Visual Basic .NET Framework 4.5.2 SOAP Web Service (ASMX)
- Migrated to: C# .NET 8 with CoreWCF

## Key Changes

1. Replaced ASMX-based web service with CoreWCF
2. Converted VB.NET code to C#
3. Updated project structure to match .NET 8 standards
4. Replaced Web.config with appsettings.json
5. Updated service endpoints to maintain backward compatibility

## Running the Application

1. Ensure you have .NET 8 SDK installed
2. Navigate to the project directory
3. Run `dotnet build` to build the project
4. Run `dotnet run` to start the service

The SOAP service will be available at:
- http://localhost:5000/GetDataService.svc
- https://localhost:5001/GetDataService.svc

## Testing the Service

You can access the service metadata (WSDL) at:
- http://localhost:5000/GetDataService

## Implemented Operations

- HelloWorld()
- GetData(string name)
- GetDataSet()
- GetReport(ReportInput reportInput)