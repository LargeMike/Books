namespace BooksLibrary.Models;

public class Book
{
   public Guid Id { get; set; }
   public string Title { get; set; }
   public int NumberOfPages { get; set; }
   public Guid GenreId { get; set; }
   public Guid AuthorId { get; set; }
   public Guid PublisherId { get; set; }
   public DateTime? DatePublished { get; set; }
   public string? NotParsedDate { get; set; }

   public Author Author { get; set; }
   public Publisher Publisher { get; set; }
   public Genre Genre { get; set; }
}