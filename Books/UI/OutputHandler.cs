using BooksLibrary.Models;
using Books.Resources;
using Microsoft.Data.SqlClient;

namespace Books.UI;

public class OutputHandler
{
   private Dictionary<string, Action> _outputMethods;
   private readonly List<Book> _books;
   private readonly List<string> _addedBooks;
   private readonly List<string> _notParsedDates;
   private readonly List<string> _duplicates;
   private string _outputFilePath;

   public OutputHandler(List<Book> books, List<string> addedBooks, List<string> notParsedDates, List<string> duplicates)
   {
      _books = books;
      _addedBooks = addedBooks;
      _notParsedDates = notParsedDates;
      _duplicates = duplicates;
      _outputFilePath = String.Empty;
   }

   public void DisplayResults()
   {
      Console.WriteLine();
   }
   
   private void InitializeOutputMethods()
   {
      _outputMethods = new Dictionary<string, Action>()
      {
         { "1", DisplayResultsNumber },
         { "2", DisplayTitles },
         { "3", DisplayAddedBooks },
         { "4", DisplayDuplicates },
         { "5", DisplayNotParsedDates },
      };
   }
   private void DisplayTitles()
   {
      var books = _books;
      Console.WriteLine($"{Messages.BookList}");
      foreach (var book in books)
      {
         Console.WriteLine(book);
      }
   }
   private void DisplayResultsNumber()
   {
      var books = _books;
      Console.WriteLine($"{Messages.CountBooksAdded}");
      Console.WriteLine($"{books.Count}");
   }
   
   private void DisplayDuplicates()
   {
      var books = _duplicates;
      Console.WriteLine($"{Messages.DuplicatedBooks}");
      Console.WriteLine($"{books.Count}");
      foreach (var book in books)
      {
         Console.WriteLine(book);
      }
   }
   
   private void DisplayAddedBooks()
   {
      var books = _addedBooks;
      Console.WriteLine($"{Messages.AddedBooks}");
      Console.WriteLine($"{books.Count}");
      foreach (var book in books)
      {
         Console.WriteLine(book);
      }
   }

   private void DisplayNotParsedDates()
   {
      var books = _notParsedDates;
      Console.WriteLine($"{Messages.NotParsedDate}");
      Console.WriteLine($"{books.Count}");
      foreach (var book in books)
      {
         Console.WriteLine(book);
      }
   }
   
   private void WriteToFile()
   {
      var path = Path.Combine(Directory.GetCurrentDirectory(), "Results");
      Directory.CreateDirectory(path);
      _outputFilePath = Path.Combine(path, $"result_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
   }

   private async Task WriteResultsToFileAsync()
   {
      WriteToFile();
      var header = "Title,Author,Genre,Publisher,NumberOfPages,DatePublished";
      var bookStrings = _books.Select(book => string.Join(",", book.Title, book.Author.AuthorName, book.Genre.GenreName,
         book.Publisher.PublisherName, book.NumberOfPages, book.DatePublished.ToString("yyyy-MM-dd")));
      List<string> rows = new List<string>();
      rows.Add(header);
      rows.AddRange(bookStrings);

      await File.WriteAllLinesAsync(_outputFilePath, rows);
   }
}