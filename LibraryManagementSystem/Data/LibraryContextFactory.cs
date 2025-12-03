using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LibraryManagementSystem.Data
{
    public class LibraryContextFactory : IDesignTimeDbContextFactory<LibraryContext>
    {
        public LibraryContext CreateDbContext(string[] args)
        {
            //Find appsettings.json using startup project
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../LibraryManagementSystemAPIs");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'.");

            var optionsBuilder = new DbContextOptionsBuilder<LibraryContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new LibraryContext(optionsBuilder.Options);
        }
    }
}
