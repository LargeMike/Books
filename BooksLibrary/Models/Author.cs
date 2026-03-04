namespace BooksLibrary.Models;

public class Author
{
    public Guid Id { get; set; }
    public string AuthorName { get; set; }
    
    public List<Book> Books { get; set; }
}