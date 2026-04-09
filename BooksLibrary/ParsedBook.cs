namespace BooksLibrary;

public class ParsedBook
{
    public string Title { get; set; }
    public string AuthorName { get; set; }
    public string GenreName { get; set; }
    public string PublisherName { get; set; }
    public int NumberOfPages { get; set; }
    public DateTime DatePublished { get; set; }
}