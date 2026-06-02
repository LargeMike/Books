using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using BooksLibrary;
  
namespace Books;
  
public class BooksDbContextFactory : IDesignTimeDbContextFactory<BooksDbContext>
{
    public BooksDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("Settings/appsettings.json")
            .AddUserSecrets<Program>()
            .Build();

        var connectionString = $"Server={config["Db:Server"]};Database={config["Db:Name"]};User Id={config["Db:User"]};Password={config["Db:Password"]};TrustServerCertificate=true;";

        var options = new DbContextOptionsBuilder<BooksDbContext>()
            .UseSqlServer(connectionString)
            .Options;
  
        return new BooksDbContext(options);
    }
}