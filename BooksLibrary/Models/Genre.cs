namespace BooksLibrary.Models;

public class Genre 
{
   public Guid Id { get; set; }
   public string GenreName { get; set; }
   
   public List<Book> Books { get; set; }
}