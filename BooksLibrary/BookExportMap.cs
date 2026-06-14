using CsvHelper.Configuration;
using BooksLibrary.Models;

namespace BooksLibrary;

public sealed class BookExportMap : ClassMap<Book>
{
    public BookExportMap()
    {
        Map(x => x.Title).Name("Title");
        Map(x => x.NumberOfPages).Name("Pages");
        Map(x => x.Genre.GenreName).Name("Genre");
        Map(x => x.DatePublished.HasValue
                ? x.DatePublished.Value.ToString("yyyy-MM-dd")
                : x.NotParsedDate).Name("ReleaseDate");
        Map(x => x.Author.AuthorName).Name("Author");
        Map(x => x.Publisher.PublisherName).Name("Publisher");
    }
}