using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BackendForIDO.Data;


namespace BackendForIDO
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Build and run the host
            var host = CreateHostBuilder(args).Build();

            // Create a scope to resolve services
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Get the AppDbContext service from the scope
                    var dbContext = services.GetRequiredService<AppDbContext>();

                    // Ensure the database is created
                    dbContext.Database.EnsureCreated();
                }
                catch (System.Exception)
                {
                    // Handle any errors that occurred during database creation
                    throw;
                }
            }

            // Run the application
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Use the Startup class to configure the web application
                    webBuilder.UseStartup<Startup>();
                });
    }
}
