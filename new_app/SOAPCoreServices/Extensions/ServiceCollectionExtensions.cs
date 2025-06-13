using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SOAPCoreServices.Interfaces;
using SOAPCoreServices.Models;
using SOAPCoreServices.Services;
using SoapCore;
using System;

namespace SOAPCoreServices.Extensions
{
    /// <summary>
    /// Extension methods for service registration
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds SOAP services to the service collection
        /// </summary>
        public static IServiceCollection AddSoapServices(this IServiceCollection services)
        {
            // Register service implementation
            services.AddScoped<IDataService, DataService>();
            
            // Add SoapCore services
            services.AddSoapCore();
            services.AddSingleton<IServiceConfiguration, ServiceConfiguration>();
            
            // Add custom SOAP operation tuner
            services.AddSoapServiceOperationTuner(new SoapCoreOperationTuner());
            
            return services;
        }
        
        /// <summary>
        /// Adds health checks for the application
        /// </summary>
        public static IServiceCollection AddServiceHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "service" });
                
            return services;
        }
    }
    
    /// <summary>
    /// Custom operation tuner for SoapCore
    /// </summary>
    public class SoapCoreOperationTuner : ISoapCoreOperationTuner
    {
        public void Tune(Microsoft.AspNetCore.Http.HttpContext httpContext, object serviceInstance, 
            SoapCore.ServiceModel.OperationDescription operation)
        {
            // Add custom operation tuning logic if needed
        }
    }
}