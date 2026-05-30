using BooksLibrary.Models;
using Books.Resources;

namespace Books.UI;

public class OutputHandler
{
   public void DisplayImportSummary(
      List<string> addedBooks,
      List<string> notParsedDates,
      List<string> duplicates)
   {
      DisplayAddedBooks(addedBooks);
      DisplayNotParsedDates(notParsedDates);
      DisplayDuplicates(duplicates);
   }

   public async Task DisplaySearchResults(List<Book> books)
   {
      DisplayResultsNumber(books);
      DisplayTitles(books);

      Console.WriteLine($"{Messages.ExportCSV}");
      var choice = Console.ReadLine();
      if (choice == "y")
      {
         await WriteResultsToFileAsync(books);
      }
   }

   private void DisplayTitles(List<Book> books)
   {
      Console.WriteLine($"{Messages.BookList}");
      foreach (var book in books)
      {
         Console.WriteLine(book.Title);
      }
   }
   
   private void DisplayResultsNumber(List<Book> books)
   {
      Console.WriteLine($"{Messages.CountBooksAdded}");
      Console.WriteLine($"{books.Count}");
   }
   
   private void DisplayDuplicates(List<string> duplicates)
   {
      Console.WriteLine($"{Messages.DuplicatedBooks}");
      Console.WriteLine($"{duplicates.Count}");
      foreach (var book in duplicates)
      {
         Console.WriteLine(book);
      }
   }
   
   private void DisplayAddedBooks(List<string> addedBooks)
   {
      Console.WriteLine($"{Messages.AddedBooks}");
      Console.WriteLine($"{addedBooks.Count}");
      foreach (var book in addedBooks)
      {
         Console.WriteLine(book);
      }
   }

   private void DisplayNotParsedDates(List<string> notParsedDates)
   {
      Console.WriteLine($"{Messages.NotParsedDate}");
      Console.WriteLine($"{notParsedDates.Count}");
      foreach (var book in notParsedDates)
      {
         Console.WriteLine(book);
      }
   }

   private async Task WriteResultsToFileAsync(List<Book> books)
   {
      var path = Path.Combine(Directory.GetCurrentDirectory(), "Results");
      Directory.CreateDirectory(path);
      var outputFilePath = Path.Combine(path, $"result_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
      var header = "Title,Author,Genre,Publisher,NumberOfPages,DatePublished";
      var bookStrings = books.Select(book => string.Join(",", book.Title, book.Author.AuthorName, book.Genre.GenreName,
         book.Publisher.PublisherName, book.NumberOfPages, book.DatePublished.ToString("yyyy-MM-dd")));
      List<string> rows = new List<string>();
      rows.Add(header);
      rows.AddRange(bookStrings);

      await File.WriteAllLinesAsync(outputFilePath, rows);
   }
}