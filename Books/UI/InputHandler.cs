using Books.Resources;
using BooksLibrary;
using System.Text.Json;

namespace Books.UI;

public class InputHandler
{
    private readonly FileProcessor _userFileProcessor;
    private readonly BookSaver _bookSaver;
    private readonly BookSearcher _bookSearcher;
    private readonly OutputHandler _outputHandler;

    public InputHandler(
        FileProcessor fileProcessor,
        BookSaver bookSaver,
        BookSearcher bookSearcher,
        OutputHandler outputHandler)
    {
        _userFileProcessor = fileProcessor ?? throw new ArgumentNullException(nameof(fileProcessor));
        _bookSaver = bookSaver ?? throw new ArgumentNullException(nameof(bookSaver));
        _bookSearcher = bookSearcher ?? throw new ArgumentNullException(nameof(bookSearcher));
        _outputHandler = outputHandler ?? throw new ArgumentNullException(nameof(outputHandler));
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.WriteLine(Messages.Greetings);
            Console.WriteLine(Messages.Options);
            var choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice))
            {
                Console.WriteLine($"{Messages.EmptyChoice}");
                continue;
            }

            if (choice == "0")
            {
                break;
            }

            if (choice == "1")
            {
                await ImportFileAsync();
                _outputHandler.DisplayImportSummary(
                    _bookSaver.AddedBooks,
                    _bookSaver.NotParsedDate,
                    _bookSaver.Duplicates);
                continue;
            }

            if (choice == "2")
            {
                await SearchBooksAsync();
                continue;
            }

            if (choice != "1" && choice != "2" && choice != "0")
            {
                Console.WriteLine($"{Messages.WrongChoice}");
            }
        }
    }

    private async Task ImportFileAsync()
    {
        Console.Write(Messages.InputPrompt);
        var filePath = Console.ReadLine();

        if (!File.Exists(filePath))
        {
            Console.WriteLine(Messages.FileNotFound);
            return;
        }

        await GetFileContent(filePath);
    }

    private async Task SearchBooksAsync()
    {
        Console.Write(Messages.FilterPrompt);
        var filterPath = Console.ReadLine();

        if (!File.Exists(filterPath))
        {
            Console.WriteLine(Messages.FileNotFound);
            return;
        }

        var json = await File.ReadAllTextAsync(filterPath);
        var filter = JsonSerializer.Deserialize<BookFilter>(json);
        if (filter == null) return;

        var books = await _bookSearcher.SearchAsync(filter);
        await _outputHandler.DisplaySearchResults(books);
    }

    private async Task GetFileContent(string filePath)
    {
        var lines = File.ReadLines(filePath);

        try
        {
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    await _userFileProcessor.ProcessLineAsync(line);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine(Messages.AccessDenied);
        }
    }
}
