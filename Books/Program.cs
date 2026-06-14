using Books.UI;
using BooksLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Books.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace Books;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>()
            .Build();
        var connectionString = $"Server={config["Db:Server"]};Database={config["Db:Name"]};User Id={config["Db:User"]};Password={config["Db:Password"]};TrustServerCertificate=true;";

        var services = new ServiceCollection();
        services.AddDbContext<BooksDbContext>(opts => opts.UseSqlServer(connectionString));
        services.AddTransient<BookSaver>();
        services.AddTransient<FileProcessor>();
        services.AddTransient<BookSearcher>();
        services.AddTransient<InputHandler>();
        services.AddTransient<OutputHandler>();
        
        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BooksDbContext>();
        await db.Database.EnsureCreatedAsync();
        var inputHandler = scope.ServiceProvider.GetRequiredService<InputHandler>();
        await inputHandler.RunAsync();
    }
}