namespace BooksLibrary;

public class LineParser
{
    public ParsedBook ParseLine(string line, string[] headers)
    {
        var values = line.Split(',');
        var book = new ParsedBook();
        
        for (int i = 0; i < headers.Length; i++)
        {
            try
            {
                var property = typeof(ParsedBook).GetProperty(headers[i]);
                property.SetValue(book, Convert.ChangeType(values[i], Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType));
            }
            catch (FormatException)
            {
                book.NotParsedDate = values[i];
            }
        }

        return book;
    }
}