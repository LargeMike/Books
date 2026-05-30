using Books.UI;
using BooksLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Books.Resources;

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
        var options = new DbContextOptionsBuilder<BooksDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        
        var context = new BooksDbContext(options);
        var lineParser = new LineParser();
        var bookSaver = new BookSaver(context);
        var fileProcessor = new FileProcessor(lineParser, bookSaver);
        var bookSearcher = new BookSearcher(context);
        var outputHandler =  new OutputHandler();
        var inputHandler = new InputHandler(fileProcessor, bookSaver, bookSearcher, outputHandler);
        
        await inputHandler.RunAsync();
    }
}