# Configuration Migration: Web.config to appsettings.json

## Overview
This document explains the migration of configuration settings from the legacy ASP.NET Web.config to modern .NET 8 appsettings.json format.

## Legacy Web.config Analysis

### Original Configuration (Web.config)
```xml
<system.web>
  <authentication mode="Windows" />
  <authorization>
    <allow users="*"/>
  </authorization>
  <sessionState mode="InProc" timeout="20"/>
  <globalization requestEncoding="utf-8" responseEncoding="utf-8"/>
  <customErrors mode="RemoteOnly"/>
</system.web>
```

## Modern .NET 8 Configuration Migration

### 1. Authentication Configuration
**Legacy:** `<authentication mode="Windows" />`
**Modern:** Configured in Program.cs with `AddAuthentication()` and referenced in appsettings.json

### 2. Authorization Configuration  
**Legacy:** `<authorization><allow users="*"/></authorization>`
**Modern:** `"Authorization": { "DefaultPolicy": "AllowAnonymous" }`

### 3. Session State Configuration
**Legacy:** `<sessionState mode="InProc" timeout="20"/>`
**Modern:** Removed from appsettings.json, configured in Program.cs with:
```csharp
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

### 4. Globalization Settings
**Legacy:** `<globalization requestEncoding="utf-8" responseEncoding="utf-8"/>`
**Modern:** `"Globalization": { "RequestEncoding": "utf-8", "ResponseEncoding": "utf-8" }`

### 5. Error Handling
**Legacy:** `<customErrors mode="RemoteOnly"/>`
**Modern:** `"ErrorHandling": { "DetailedErrors": false, "IncludeExceptionDetails": false }`

## New .NET 8 Configuration Sections

### 1. Kestrel Server Configuration
```json
"Kestrel": {
  "Endpoints": {
    "Http": {
      "Url": "http://localhost:57114"
    }
  }
}
```

### 2. SOAP Endpoints Configuration
```json
"SoapEndpoints": {
  "GetDataService": {
    "Path": "/GetDataService.asmx",
    "Binding": "BasicHttpBinding",
    "EncoderOptions": {
      "MessageVersion": "Soap11"
    }
  }
}
```

### 3. CORS Configuration
```json
"Cors": {
  "EnableCors": true,
  "DefaultPolicyName": "AllowSpecificOrigins",
  "Policies": {
    "AllowSpecificOrigins": {
      "AllowedOrigins": ["http://localhost:57114"],
      "AllowedMethods": ["GET", "POST"],
      "AllowedHeaders": ["Content-Type", "SOAPAction", "Authorization"],
      "AllowCredentials": true
    }
  }
}
```

### 4. Modern Structured Logging
```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning",
    "SoapCore": "Information"
  },
  "Console": {
    "IncludeScopes": false,
    "TimestampFormat": "yyyy-MM-ddTHH:mm:ss.fffZ"
  }
}
```

## Configuration Files Structure

### appsettings.json (Base Configuration)
- Production-ready defaults
- Security-focused settings
- Basic logging levels

### appsettings.Development.json (Development Overrides)
- Enhanced debug logging
- Detailed error reporting
- Permissive CORS for development

### appsettings.Production.json (Production Overrides)
- Minimal logging for performance
- Strict security settings
- HTTPS enforcement

## Removed Legacy Sections

The following Web.config sections are no longer needed in .NET 8:

1. **system.codedom** - Compiler settings handled by .NET 8 SDK
2. **compilation** - Build configuration in project file
3. **httpRuntime** - Handled by Kestrel server
4. **pages** - Not applicable to Web API/SOAP services
5. **SessionState XML configuration** - Moved to Program.cs middleware

## Benefits of Modern Configuration

1. **Environment-specific overrides** - Automatic merging of configuration files
2. **Strongly-typed configuration** - Can bind to C# classes
3. **Hierarchical structure** - Better organization and readability
4. **JSON format** - Human-readable and tooling-friendly
5. **Runtime configuration changes** - Some settings can be changed without restart
6. **Cloud-native support** - Easy integration with configuration providers