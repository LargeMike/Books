using System.Globalization;
using CsvHelper.Configuration;

namespace BooksLibrary;

public sealed class LineParser : ClassMap<ParsedBook>
{
    public LineParser()
    {
        AutoMap(CultureInfo.InvariantCulture);
        Map(m => m.ReleaseDate).Convert(args =>
        {
            var raw = args.Row.GetField("ReleaseDate");
            return DateTime.TryParse(raw, out var date) ? date : (DateTime?)null;
        });
        Map(m => m.NotParsedDate).Convert(args =>
        {
            var raw = args.Row.GetField("ReleaseDate");
            return DateTime.TryParse(raw, out _) ? null : raw;
        });
    }
}