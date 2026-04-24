using System.Text;

namespace BooksLibrary;

public class LineParser
{
    public ParsedBook ParseLine(string line, string[] headers)
    {
        var values = SplitCsvLine(line);
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
    
    internal List<string> SplitCsvLine(string line)
    {
        var field = new StringBuilder();
        List<string> splittedLine = new List<string>();
        bool inQuote = false;

        foreach (var character in line)
        {
            if (inQuote == false)
            {
                if (character != '"' && character != ',')
                {
                    field.Append(character);
                }

                if (character == ',')
                {
                    splittedLine.Add(field.ToString());
                    field.Clear();
                }

                if (character == '"')
                {
                    inQuote = true;
                }
            }

            else if (inQuote == true)
            {
                if (character != '"')
                {
                    field.Append(character);
                }

                if (character == '"')
                {
                    inQuote = false;
                }
            }
        }
        splittedLine.Add(field.ToString());
        
        return splittedLine;
    }
}