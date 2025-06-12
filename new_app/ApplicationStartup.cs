using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(SOAPWebServicesCore.ApplicationStartup))]

namespace SOAPWebServicesCore
{
    public class ApplicationStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                // Configure application startup services
                // This replaces the Application_Start event from Global.asax.vb
            });
        }
    }
}