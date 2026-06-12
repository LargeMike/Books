using System.Globalization;
using BooksLibrary.Models;
using CsvHelper;

namespace BooksLibrary;

public class FileProcessor
{
    private readonly BookSaver _bookSaver;
    
    public FileProcessor(BookSaver bookSaver)
    {
        _bookSaver = bookSaver;
    }

    public async Task ProcessFileAsync(TextReader reader)
    {
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<LineParser>();
        await foreach (var book in csv.GetRecordsAsync<ParsedBook>())
        {
            await _bookSaver.SaveAsync(book);
        }
    }
}