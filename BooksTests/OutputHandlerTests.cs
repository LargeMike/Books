using Books.UI;
using BooksLibrary.Models;

namespace BooksTests;

[TestClass]
public class OutputHandlerTests
{
    private TextWriter _originalOut = null!;
    private TextReader _originalIn = null!;
    private StringWriter _capturedOutput = null!;

    [TestInitialize]
    public void Setup()
    {
        _originalOut = Console.Out;
        _originalIn = Console.In;
        _capturedOutput = new StringWriter();
        Console.SetOut(_capturedOutput);
    }

    [TestCleanup]
    public void Cleanup()
    {
        Console.SetOut(_originalOut);
        Console.SetIn(_originalIn);
        _capturedOutput.Dispose();
    }

    [TestMethod]
    public void DisplayImportSummary_ExitChoice_WritesCountsAndExits()
    {
        Console.SetIn(new StringReader("0\n"));
        var handler = new OutputHandler();
        var added = new List<string> { "Book A", "Book B" };
        var notParsed = new List<string> { "Book C" };
        var duplicates = new List<string>();

        handler.DisplayImportSummary(added, notParsed, duplicates);

        var output = _capturedOutput.ToString();
        Assert.Contains("2", output);
        Assert.Contains("1", output);
    }

    [TestMethod]
    public void DisplayImportSummary_ChoiceOne_WritesAddedBookTitles()
    {
        Console.SetIn(new StringReader("1\n0\n"));
        var handler = new OutputHandler();
        var added = new List<string> { "The Great Gatsby" };

        handler.DisplayImportSummary(added, new List<string>(), new List<string>());

        Assert.Contains("The Great Gatsby", _capturedOutput.ToString());
    }

    [TestMethod]
    public void DisplayImportSummary_ChoiceTwo_WritesNotParsedDateTitles()
    {
        Console.SetIn(new StringReader("2\n0\n"));
        var handler = new OutputHandler();
        var notParsed = new List<string> { "Unknown Date Book" };

        handler.DisplayImportSummary(new List<string>(), notParsed, new List<string>());

        Assert.Contains("Unknown Date Book", _capturedOutput.ToString());
    }

    [TestMethod]
    public void DisplayImportSummary_ChoiceThree_WritesDuplicateTitles()
    {
        Console.SetIn(new StringReader("3\n0\n"));
        var handler = new OutputHandler();
        var duplicates = new List<string> { "Duplicate Book" };

        handler.DisplayImportSummary(new List<string>(), new List<string>(), duplicates);

        Assert.Contains("Duplicate Book", _capturedOutput.ToString());
    }

    [TestMethod]
    public async Task DisplaySearchResults_WithResults_DisplaysBookTitles()
    {
        Console.SetIn(new StringReader("n\n"));
        var handler = new OutputHandler();
        var books = new List<Book>
        {
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Dune",
                NumberOfPages = 412,
                DatePublished = new DateTime(1965, 8, 1),
                Author = new Author { Id = Guid.NewGuid(), AuthorName = "Frank Herbert" },
                Genre = new Genre { Id = Guid.NewGuid(), GenreName = "Sci-Fi" },
                Publisher = new Publisher { Id = Guid.NewGuid(), PublisherName = "Chilton Books" }
            }
        };

        await handler.DisplaySearchResults(books);

        Assert.Contains("Dune", _capturedOutput.ToString());
    }
}
