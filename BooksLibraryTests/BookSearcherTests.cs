using BooksLibrary;
using BooksLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksLibraryTests;

[TestClass]
public class BookSearcherTests
{
    private static BooksDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<BooksDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new BooksDbContext(options);
    }

    private static async Task SeedDatabaseAsync(BooksDbContext context)
    {
        var rowling = new Author { Id = Guid.NewGuid(), AuthorName = "J.K. Rowling" };
        var tolkien = new Author { Id = Guid.NewGuid(), AuthorName = "J.R.R. Tolkien" };
        var martin = new Author { Id = Guid.NewGuid(), AuthorName = "Robert Martin" };

        var fantasy = new Genre { Id = Guid.NewGuid(), GenreName = "Fantasy" };
        var programming = new Genre { Id = Guid.NewGuid(), GenreName = "Programming" };

        var bloomsbury = new Publisher { Id = Guid.NewGuid(), PublisherName = "Bloomsbury" };
        var oreilly = new Publisher { Id = Guid.NewGuid(), PublisherName = "O'Reilly" };

        context.Authors.AddRange(rowling, tolkien, martin);
        context.Genres.AddRange(fantasy, programming);
        context.Publishers.AddRange(bloomsbury, oreilly);

        context.Books.AddRange(
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Harry Potter and the Sorcerer's Stone",
                NumberOfPages = 320,
                DatePublished = new DateTime(1997, 6, 26),
                AuthorId = rowling.Id,
                GenreId = fantasy.Id,
                PublisherId = bloomsbury.Id
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Harry Potter and the Chamber of Secrets",
                NumberOfPages = 251,
                DatePublished = new DateTime(1998, 7, 2),
                AuthorId = rowling.Id,
                GenreId = fantasy.Id,
                PublisherId = bloomsbury.Id
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "The Lord of the Rings",
                NumberOfPages = 1178,
                DatePublished = new DateTime(1954, 7, 29),
                AuthorId = tolkien.Id,
                GenreId = fantasy.Id,
                PublisherId = bloomsbury.Id
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Clean Code",
                NumberOfPages = 431,
                DatePublished = new DateTime(2008, 8, 1),
                AuthorId = martin.Id,
                GenreId = programming.Id,
                PublisherId = oreilly.Id
            }
        );

        await context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task SearchAsync_EmptyFilter_ReturnsAllBooks()
    {
        using var context = CreateContext();
        await SeedDatabaseAsync(context);
        var searcher = new BookSearcher(context);

        var result = await searcher.SearchAsync(new BookFilter());

        Assert.HasCount(4, result);
    }

    [TestMethod]
    public async Task SearchAsync_FilterByTitle_ReturnsMatchingBooks()
    {
        using var context = CreateContext();
        await SeedDatabaseAsync(context);
        var searcher = new BookSearcher(context);

        var result = await searcher.SearchAsync(new BookFilter { Title = "Harry" });

        Assert.HasCount(2, result);
        Assert.IsTrue(result.All(b => b.Title.Contains("Harry")));
    }

    [TestMethod]
    public async Task SearchAsync_FilterByAuthor_ReturnsMatchingBooks()
    {
        using var context = CreateContext();
        await SeedDatabaseAsync(context);
        var searcher = new BookSearcher(context);

        var result = await searcher.SearchAsync(new BookFilter { Author = "J.K. Rowling" });

        Assert.HasCount(2, result);
        Assert.IsTrue(result.All(b => b.Author.AuthorName == "J.K. Rowling"));
    }

    [TestMethod]
    public async Task SearchAsync_FilterByGenre_ReturnsMatchingBooks()
    {
        using var context = CreateContext();
        await SeedDatabaseAsync(context);
        var searcher = new BookSearcher(context);

        var result = await searcher.SearchAsync(new BookFilter { Genre = "Fantasy" });

        Assert.HasCount(3, result);
        Assert.IsTrue(result.All(b => b.Genre.GenreName == "Fantasy"));
    }

    [TestMethod]
    public async Task SearchAsync_FilterByPublisher_ReturnsMatchingBooks()
    {
        using var context = CreateContext();
        await SeedDatabaseAsync(context);
        var searcher = new BookSearcher(context);

        var result = await searcher.SearchAsync(new BookFilter { Publisher = "O'Reilly" });

        Assert.HasCount(1, result);
        Assert.AreEqual("Clean Code", result[0].Title);
    }

    [TestMethod]
    public async Task SearchAsync_MoreThanPages_ReturnsMatchingBooks()
    {
        using var context = CreateContext();
        await SeedDatabaseAsync(context);
        var searcher = new BookSearcher(context);

        var result = await searcher.SearchAsync(new BookFilter { MoreThanPages = 400 });

        Assert.HasCount(2, result);
        Assert.IsTrue(result.All(b => b.NumberOfPages > 400));
    }

    [TestMethod]
    public async Task SearchAsync_LessThanPages_ReturnsMatchingBooks()
    {
        using var context = CreateContext();
        await SeedDatabaseAsync(context);
        var searcher = new BookSearcher(context);

        var result = await searcher.SearchAsync(new BookFilter { LessThanPages = 300 });

        Assert.HasCount(1, result);
        Assert.AreEqual("Harry Potter and the Chamber of Secrets", result[0].Title);
    }

    [TestMethod]
    public async Task SearchAsync_PublishedAfter_ReturnsMatchingBooks()
    {
        using var context = CreateContext();
        await SeedDatabaseAsync(context);
        var searcher = new BookSearcher(context);

        var result = await searcher.SearchAsync(new BookFilter { PublishedAfter = new DateTime(2000, 1, 1) });

        Assert.HasCount(1, result);
        Assert.AreEqual("Clean Code", result[0].Title);
    }

    [TestMethod]
    public async Task SearchAsync_PublishedBefore_ReturnsMatchingBooks()
    {
        using var context = CreateContext();
        await SeedDatabaseAsync(context);
        var searcher = new BookSearcher(context);

        var result = await searcher.SearchAsync(new BookFilter { PublishedBefore = new DateTime(1960, 1, 1) });

        Assert.HasCount(1, result);
        Assert.AreEqual("The Lord of the Rings", result[0].Title);
    }

    [TestMethod]
    public async Task SearchAsync_EmptyFilter_IncludesNullDateBooks()
    {
        using var context = CreateContext();
        await SeedDatabaseAsync(context);
        var unknown = new Author { Id = Guid.NewGuid(), AuthorName = "Unknown Author" };
        var genre = new Genre { Id = Guid.NewGuid(), GenreName = "Unknown" };
        var publisher = new Publisher { Id = Guid.NewGuid(), PublisherName = "Unknown" };
        context.Authors.Add(unknown);
        context.Genres.Add(genre);
        context.Publishers.Add(publisher);
        context.Books.Add(new Book
        {
            Id = Guid.NewGuid(),
            Title = "Ancient Book",
            NumberOfPages = 100,
            DatePublished = null,
            NotParsedDate = "рік невідомий",
            AuthorId = unknown.Id,
            GenreId = genre.Id,
            PublisherId = publisher.Id
        });
        await context.SaveChangesAsync();
        var searcher = new BookSearcher(context);

        var result = await searcher.SearchAsync(new BookFilter());

        Assert.HasCount(5, result);
    }

    [TestMethod]
    public async Task SearchAsync_CombinedAuthorAndGenreFilter_ReturnsOnlyMatching()
    {
        using var context = CreateContext();
        await SeedDatabaseAsync(context);
        var searcher = new BookSearcher(context);

        var result = await searcher.SearchAsync(new BookFilter
        {
            Author = "J.K. Rowling",
            Genre = "Fantasy"
        });

        Assert.HasCount(2, result);
        Assert.IsTrue(result.All(b => b.Author.AuthorName == "J.K. Rowling"));
    }
}
