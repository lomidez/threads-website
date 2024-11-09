using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ContosoCrafts.WebSite
{
    // Entry point for the application
    public class Program
    {
        // Main method, which is the entry point of the application
        public static void Main(string[] args)
        {
            // Builds and runs the application host
            CreateHostBuilder(args).Build().Run();
        }

        // Creates and configures the web host builder
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args) // Create a default host builder with configuration
                .ConfigureWebHostDefaults(webBuilder => // Configure web host defaults
                {
                    // Specifies the Startup class to configure services and middleware
                    webBuilder.UseStartup<Startup>();
                });
    }
}
