namespace BooksLibrary;

public class LineParser
{
    public ParsedBook ParseLine(string line, string[] headers)
    {
        var values = line.Split(',');
        var book = new ParsedBook();
        var property = typeof(ParsedBook).GetProperty(nameof(ParsedBook.Title));
        
        return book;
    }
}