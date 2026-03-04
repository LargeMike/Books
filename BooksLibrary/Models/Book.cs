namespace BooksLibrary.Models;

public class Book
{
   public Guid Id { get; set; }
   public string Title { get; set; }
   public int NumberOfPages { get; set; }
   public Guid GenreId { get; set; }
   public Guid AuthorId { get; set; }
   public Guid PublisherId { get; set; }
   public DateTime DatePublished { get; set; }
   
   public List<Author> Authors { get; set; }
   public List<Publisher> Publishers { get; set; }
   public List<Genre> Genres { get; set; }
}