using Books.UI;
using BooksLibrary;
using Microsoft.EntityFrameworkCore;

namespace BooksTests;

[TestClass]
public class InputHandlerTests
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

    private static InputHandler CreateHandler()
    {
        var options = new DbContextOptionsBuilder<BooksDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new BooksDbContext(options);
        var bookSaver = new BookSaver(context);
        var fileProcessor = new FileProcessor(bookSaver);
        var bookSearcher = new BookSearcher(context);
        var outputHandler = new OutputHandler();
        return new InputHandler(fileProcessor, bookSaver, bookSearcher, outputHandler);
    }

    [TestMethod]
    public async Task RunAsync_ExitChoice_BreaksLoop()
    {
        Console.SetIn(new StringReader("0\n"));
        var handler = CreateHandler();

        await handler.RunAsync();

        // If we reach here — the loop exited correctly
    }

    [TestMethod]
    public async Task RunAsync_EmptyInput_WritesEmptyChoiceMessage()
    {
        Console.SetIn(new StringReader("\n0\n"));
        var handler = CreateHandler();

        await handler.RunAsync();

        Assert.Contains("No option selected", _capturedOutput.ToString());
    }

    [TestMethod]
    public async Task RunAsync_WrongChoice_WritesWrongChoiceMessage()
    {
        Console.SetIn(new StringReader("9\n0\n"));
        var handler = CreateHandler();

        await handler.RunAsync();

        Assert.Contains("Invalid option", _capturedOutput.ToString());
    }

    [TestMethod]
    public async Task RunAsync_ImportNonExistentFile_WritesFileNotFound()
    {
        // "1" → import, nonexistent path → FileNotFound, "0" → exit DisplayImportSummary, "0" → exit RunAsync
        Console.SetIn(new StringReader("1\n/nonexistent/path/file.csv\n0\n0\n"));
        var handler = CreateHandler();

        await handler.RunAsync();

        Assert.Contains("File not found", _capturedOutput.ToString());
    }

    [TestMethod]
    public async Task RunAsync_SearchNonExistentFilterFile_WritesFileNotFound()
    {
        // "2" → search, nonexistent path → FileNotFound, "0" → exit RunAsync
        Console.SetIn(new StringReader("2\n/nonexistent/filter.json\n0\n"));
        var handler = CreateHandler();

        await handler.RunAsync();

        Assert.Contains("File not found", _capturedOutput.ToString());
    }

    [TestMethod]
    public async Task RunAsync_ImportUnreadableFile_WritesAccessDenied()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "Title,Author\nSome Book,Some Author");
#pragma warning disable CA1416
            File.SetUnixFileMode(tempFile, UnixFileMode.None);
#pragma warning restore CA1416

            // "1" → import, tempFile → exists but unreadable → AccessDenied, "0" → exit DisplayImportSummary, "0" → exit RunAsync
            Console.SetIn(new StringReader($"1\n{tempFile}\n0\n0\n"));
            var handler = CreateHandler();

            await handler.RunAsync();

            Assert.Contains("Access denied", _capturedOutput.ToString());
        }
        finally
        {
#pragma warning disable CA1416
            File.SetUnixFileMode(tempFile, UnixFileMode.UserRead | UnixFileMode.UserWrite);
#pragma warning restore CA1416
            File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void Constructor_NullFileProcessor_ThrowsArgumentNullException()
    {
        var options = new DbContextOptionsBuilder<BooksDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new BooksDbContext(options);
        var bookSaver = new BookSaver(context);
        var bookSearcher = new BookSearcher(context);
        var outputHandler = new OutputHandler();

        Assert.Throws<ArgumentNullException>(() =>
            new InputHandler(null!, bookSaver, bookSearcher, outputHandler));
    }

    [TestMethod]
    public void Constructor_NullBookSaver_ThrowsArgumentNullException()
    {
        var options = new DbContextOptionsBuilder<BooksDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new BooksDbContext(options);
        var bookSaver = new BookSaver(context);
        var fileProcessor = new FileProcessor(bookSaver);
        var bookSearcher = new BookSearcher(context);
        var outputHandler = new OutputHandler();

        Assert.Throws<ArgumentNullException>(() =>
            new InputHandler(fileProcessor, null!, bookSearcher, outputHandler));
    }
}
