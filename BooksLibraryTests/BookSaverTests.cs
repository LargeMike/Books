using BooksLibrary;
using Microsoft.EntityFrameworkCore;

namespace BooksLibraryTests;

[TestClass]
public class BookSaverTests
{
    private static BooksDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<BooksDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new BooksDbContext(options);
    }

    private static ParsedBook ValidBook(string title = "Test Book") => new()
    {
        Title = title,
        Author = "Test Author",
        Genre = "Test Genre",
        Publisher = "Test Publisher",
        Pages = 200,
        ReleaseDate = new DateTime(2020, 1, 1)
    };

    [TestMethod]
    public async Task SaveAsync_ValidBook_AddsToAddedBooks()
    {
        using var context = CreateContext();
        var saver = new BookSaver(context);

        await saver.SaveAsync(ValidBook());

        Assert.HasCount(1, saver.AddedBooks);
        Assert.AreEqual("Test Book", saver.AddedBooks[0]);
    }

    [TestMethod]
    public async Task SaveAsync_ValidBook_PersistsToDatabase()
    {
        using var context = CreateContext();
        var saver = new BookSaver(context);

        await saver.SaveAsync(ValidBook());

        Assert.AreEqual(1, await context.Books.CountAsync());
    }

    [TestMethod]
    public async Task SaveAsync_NullReleaseDate_AddsToNotParsedDate()
    {
        using var context = CreateContext();
        var saver = new BookSaver(context);
        var book = ValidBook();
        book.ReleaseDate = null;
        book.NotParsedDate = "невідома дата";

        await saver.SaveAsync(book);

        Assert.HasCount(1, saver.NotParsedDate);
        Assert.AreEqual(1, await context.Books.CountAsync());
        var saved = await context.Books.FirstAsync();
        Assert.IsNull(saved.DatePublished);
        Assert.AreEqual("невідома дата", saved.NotParsedDate);
    }

    [TestMethod]
    public async Task SaveAsync_TwoBooksWithSameNotParsedDate_DetectsDuplicate()
    {
        using var context = CreateContext();
        var saver = new BookSaver(context);
        var book = ValidBook();
        book.ReleaseDate = null;
        book.NotParsedDate = "невідома дата";

        await saver.SaveAsync(book);
        await saver.SaveAsync(book);

        Assert.HasCount(1, saver.Duplicates);
        Assert.AreEqual(1, await context.Books.CountAsync());
    }

    [TestMethod]
    public async Task SaveAsync_DuplicateBook_AddsToDuplicates()
    {
        using var context = CreateContext();
        var saver = new BookSaver(context);

        await saver.SaveAsync(ValidBook());
        await saver.SaveAsync(ValidBook());

        Assert.HasCount(1, saver.Duplicates);
        Assert.HasCount(1, saver.AddedBooks);
    }

    [TestMethod]
    public async Task SaveAsync_NewAuthor_CreatesAuthorInDatabase()
    {
        using var context = CreateContext();
        var saver = new BookSaver(context);

        await saver.SaveAsync(ValidBook());

        Assert.AreEqual(1, await context.Authors.CountAsync());
    }

    [TestMethod]
    public async Task SaveAsync_TwoBooksWithSameAuthor_ReusesAuthor()
    {
        using var context = CreateContext();
        var saver = new BookSaver(context);

        await saver.SaveAsync(ValidBook("Book One"));
        await saver.SaveAsync(ValidBook("Book Two"));

        Assert.AreEqual(1, await context.Authors.CountAsync());
        Assert.AreEqual(2, await context.Books.CountAsync());
    }
}
