using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ContosoCrafts.WebSite.Services;

namespace ContosoCrafts.WebSite
{
    // Startup class used for configuring services and the application's HTTP request pipeline
    public class Startup
    {
        // Constructor to initialize the Startup class with configuration
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Property to hold the configuration for the application
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Adds Razor Pages with runtime compilation support for dynamic page updates
            services.AddRazorPages().AddRazorRuntimeCompilation();

            // Adds support for Blazor server-side components
            services.AddServerSideBlazor();

            // Adds HTTP client services for making HTTP requests
            services.AddHttpClient();

            // Adds support for controllers (MVC or API controllers)
            services.AddControllers();

            // Registers JsonFileProductService as a transient service for dependency injection
            services.AddTransient<JsonFileProductService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the development environment (more detailed error pages)
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Configure the production environment (generic error pages)
                app.UseExceptionHandler("/Error");

                // HSTS (HTTP Strict Transport Security) configuration for secure HTTPS redirection
                // The default HSTS value is 30 days. This can be adjusted for production use.
                app.UseHsts();
            }

            // Redirect HTTP requests to HTTPS
            app.UseHttpsRedirection();

            // Serve static files (e.g., CSS, JavaScript)
            app.UseStaticFiles();

            // Enable routing capabilities to match requests to endpoints
            app.UseRouting();

            // Enable authorization middleware to enforce security policies
            app.UseAuthorization();

            // Configure endpoints for Razor Pages, Controllers, and Blazor components
            app.UseEndpoints(endpoints =>
            {
                // Map Razor Pages to handle page requests
                endpoints.MapRazorPages();

                // Map API Controller routes
                endpoints.MapControllers();

                // Map the Blazor SignalR Hub for server-side Blazor communication
                endpoints.MapBlazorHub();

                // Example commented out endpoint for custom route handling
                // endpoints.MapGet("/products", (context) => 
                // {
                //     var products = app.ApplicationServices.GetService<JsonFileProductService>().GetProducts();
                //     var json = JsonSerializer.Serialize<IEnumerable<Product>>(products);
                //     return context.Response.WriteAsync(json);
                // });
            });
        }
    }
}
