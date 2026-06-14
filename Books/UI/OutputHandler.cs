using BooksLibrary;
using BooksLibrary.Models;
using Books.Resources;
using CsvHelper;
using System.Globalization;

namespace Books.UI;

public class OutputHandler
{
   public void DisplayImportSummary(
      List<string> addedBooks,
      List<string> notParsedDates,
      List<string> duplicates)
   {
      while (true)
      {
         Console.WriteLine(Messages.OutputMessage, addedBooks.Count, notParsedDates.Count, duplicates.Count);
         Console.WriteLine($"{Messages.OutputMethods}");
         var choice = Console.ReadLine();
         
         if (choice == "1")
         {
            DisplayAddedBooks(addedBooks);
         }

         if (choice == "2")
         {
            DisplayNotParsedDates(notParsedDates);
         }

         if (choice == "3")
         {
            DisplayDuplicates(duplicates);
         }

         if (choice == "0")
         {
            return;
         }
      }
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

      await using var writer = new StreamWriter(outputFilePath);
      await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
      csv.Context.RegisterClassMap<BookExportMap>();
      await csv.WriteRecordsAsync(books);
   }
}