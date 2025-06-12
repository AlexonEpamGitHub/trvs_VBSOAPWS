# SOAP Web Service - .NET 8

## Overview
This is a modern SOAP web service built with ASP.NET Core 8 and SoapCore, migrated from a legacy VB.NET application. The service provides backward compatibility while leveraging modern .NET 8 features and performance improvements.

## Architecture
- **Framework**: ASP.NET Core 8
- **SOAP Implementation**: SoapCore library
- **Language**: C#
- **Original Source**: VB.NET .NET Framework 4.5.2 ASMX Web Service

## Endpoints

### SOAP Endpoints
- **`/GetDataService.asmx`** - Main SOAP service endpoint for all web service operations
- **`/GetDataService.asmx?wsdl`** - WSDL (Web Services Description Language) definition endpoint for service metadata

### REST Endpoints
- **`/api/health`** - Health check endpoint for service monitoring and availability verification
- **`/swagger`** - Interactive API documentation (available in Development environment only)

## Available SOAP Methods

### Method Signatures
- **`HelloWorld()`**
  - Returns: `string`
  - Description: Returns a simple "Hello World" greeting message

- **`GetData(string name)`**
  - Parameters: `name` (string) - Name for personalized greeting
  - Returns: `string`
  - Description: Returns a personalized greeting message using the provided name

- **`GetDataSet()`**
  - Returns: `DataSet`
  - Description: Returns a sample DataSet with demonstration data

- **`GetReport(ReportInput input)`**
  - Parameters: `input` (ReportInput) - Complex input object containing report parameters
  - Returns: `ReportOutput`
  - Description: Processes report request and returns formatted report data

## Running the Service

### Prerequisites
- .NET 8 SDK installed
- Compatible operating system (Windows, Linux, macOS)

### Start the Service
```bash
dotnet run
```

### Service Availability
The service will be available at: **http://localhost:57114**

## Migration Details

### Source Configuration
- **Original Framework**: VB.NET .NET Framework 4.5.2
- **Original Type**: ASMX Web Service
- **Legacy Port**: 57114

### Target Configuration
- **New Framework**: C# .NET 8 ASP.NET Core
- **SOAP Library**: SoapCore
- **Preserved Port**: 57114 (maintained for backward compatibility)
- **Authentication**: Configured for Windows authentication compatibility

### Migration Benefits
- Improved performance and scalability
- Cross-platform compatibility
- Modern development tooling
- Enhanced security features
- Better monitoring and diagnostics

## Development Features

### Development Environment
- **Swagger UI**: Available at `/swagger` endpoint for interactive API testing
- **Hot Reload**: Supported for rapid development cycles
- **Debugging**: Full debugging support with Visual Studio and VS Code

### Testing
- Health check endpoint for automated monitoring
- WSDL validation for SOAP contract verification
- Swagger UI for manual testing in development

## Configuration

### Environment-Specific Features
- **Development**: Swagger UI enabled, detailed error messages
- **Production**: Security-hardened, minimal error exposure

### Authentication
- Windows authentication support maintained from legacy system
- Configurable authentication schemes
- Backward compatibility with existing client applications

## Monitoring and Maintenance

### Health Checks
Use the `/api/health` endpoint to verify service status:
```bash
curl http://localhost:57114/api/health
```

### WSDL Verification
Access the WSDL definition to verify service contract:
```
http://localhost:57114/GetDataService.asmx?wsdl
```

## Client Integration

### SOAP Client Configuration
Clients can consume this service using the WSDL definition at:
```
http://localhost:57114/GetDataService.asmx?wsdl
```

### Legacy Compatibility
The service maintains full backward compatibility with existing VB.NET client applications while providing modern performance and features.