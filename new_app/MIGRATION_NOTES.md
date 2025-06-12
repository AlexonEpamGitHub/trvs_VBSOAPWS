# .NET 8 SOAP Web Service Migration - Development Configuration

## Enhanced appsettings.Development.json Configuration

This file provides comprehensive development-specific configuration for the migrated .NET 8 SOAP web service.

### Key Features:

#### 1. **Granular Logging Configuration**
- **Application Level**: `Information` for general application logging
- **Framework Components**: 
  - `Microsoft.AspNetCore.*`: `Information` to `Debug` levels for detailed framework debugging
  - `SoapCore`: `Debug` level for SOAP-specific debugging
  - `SOAPWebServicesCore`: `Debug` level for application-specific debugging
- **System Components**: `Warning` level to reduce noise from system logging

#### 2. **Development-Specific Features**
- **DetailedErrors**: `true` for enhanced error information during development
- **DeveloperExceptionPage**: Enabled for detailed exception information
- **BrowserLink**: Disabled (not needed for SOAP services)
- **DatabaseErrorPage**: Disabled (not applicable for this service)

#### 3. **Authentication & Session Configuration**
- **RequireHttpsMetadata**: `false` for development flexibility
- **Session Timeout**: Extended to 30 minutes for development convenience
- **Validation**: Relaxed for development environment

#### 4. **Migration from Legacy Configuration**
This configuration replaces the legacy `Web.Debug.config` transformation approach with modern JSON-based configuration following .NET 8 standards.

### Legacy vs Modern Configuration Mapping:

| Legacy (Web.config/Web.Debug.config) | Modern (.NET 8 appsettings.Development.json) |
|---------------------------------------|-----------------------------------------------|
| `compilation debug="true"` | `DetailedErrors: true` + granular logging |
| `customErrors mode="RemoteOnly"` | `DeveloperExceptionPage: true` |
| `authentication mode="Windows"` | `Authentication.RequireHttpsMetadata: false` |
| `sessionState timeout="20"` | `Session.Timeout: 30` (extended for dev) |
| XML transformations | Direct JSON configuration overrides |

### Benefits:
- **Enhanced Debugging**: Granular control over logging levels
- **Performance Optimized**: Reduced logging noise from system components
- **Modern Standards**: Follows .NET 8 configuration best practices
- **Development Friendly**: Extended timeouts and detailed error reporting
- **Maintainable**: JSON format is more readable than XML transformations