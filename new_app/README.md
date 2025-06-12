# SOAP Web Service - .NET 8

## Overview
This is a modern SOAP web service built with ASP.NET Core 8 and SoapCore, migrated from a legacy VB.NET application.

## Endpoints
- `/GetDataService.asmx` - Main SOAP service endpoint
- `/GetDataService.asmx?wsdl` - WSDL definition
- `/api/health` - Health check endpoint
- `/swagger` - API documentation (development only)

## Available SOAP Methods
- `HelloWorld()` - Returns "Hello World"
- `GetData(string name)` - Returns personalized greeting
- `GetDataSet()` - Returns sample DataSet
- `GetReport(ReportInput input)` - Returns report data

## Running the Service
```bash
dotnet run
```

Service will be available at: http://localhost:57114

## Migration Notes
This service has been migrated from:
- **Source**: VB.NET .NET Framework 4.5.2 ASMX Web Service
- **Target**: C# .NET 8 ASP.NET Core with SoapCore
- **Port**: 57114 (preserved from legacy application)
- **Authentication**: Configured for Windows authentication compatibility

## Development
The service includes Swagger UI for development and testing at `/swagger` endpoint when running in Development environment.