using BooksLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksLibrary;

public class BookSaver
{
    private readonly BooksDbContext _dbContext;
    public List<string> Duplicates { get; private set; }
    public List<string> AddedBooks { get; private set; }
    public List<string> NotParsedDate { get; private set; }

    public BookSaver(BooksDbContext dbContext)
    {
        _dbContext = dbContext;
        Duplicates = new List<string>();
        AddedBooks = new List<string>();
        NotParsedDate = new List<string>();
    }

    public async Task SaveAsync(ParsedBook parsedBook)
    {
        Duplicates.Clear();
        AddedBooks.Clear();
        NotParsedDate.Clear();
        
        if (parsedBook.ReleaseDate == null)
            NotParsedDate.Add(parsedBook.Title);

        var author = await FindOrCreateAuthorAsync(parsedBook.Author);
        var genre = await FindOrCreateGenreAsync(parsedBook.Genre);
        var publisher = await FindOrCreatePublisherAsync(parsedBook.Publisher);

        bool isDuplicate;
        if (parsedBook.ReleaseDate != null)
            isDuplicate = await _dbContext.Books.AnyAsync(b =>
                b.Title == parsedBook.Title &&
                b.AuthorId == author.Id &&
                b.PublisherId == publisher.Id &&
                b.DatePublished == parsedBook.ReleaseDate);
        else
            isDuplicate = await _dbContext.Books.AnyAsync(b =>
                b.Title == parsedBook.Title &&
                b.AuthorId == author.Id &&
                b.PublisherId == publisher.Id &&
                b.NotParsedDate == parsedBook.NotParsedDate);

        if (isDuplicate)
        {
            Duplicates.Add(parsedBook.Title);
            return;
        }

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = parsedBook.Title,
            NumberOfPages = parsedBook.Pages,
            AuthorId = author.Id,
            GenreId = genre.Id,
            PublisherId = publisher.Id,
            DatePublished = parsedBook.ReleaseDate,
            NotParsedDate = parsedBook.NotParsedDate,
        };
        
        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();
        AddedBooks.Add(parsedBook.Title);
    }

    private async Task<Author> FindOrCreateAuthorAsync(string authorName)
    {
        var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.AuthorName == authorName);
        if (author == null)
        {
            author = new Author {Id = Guid.NewGuid(), AuthorName =  authorName};
            _dbContext.Authors.Add(author);
            await _dbContext.SaveChangesAsync();
        }

        return author;
    }

    private async Task<Genre> FindOrCreateGenreAsync(string genreName)
    {
        var genre = await _dbContext.Genres.FirstOrDefaultAsync(g => g.GenreName == genreName);
        if (genre == null)
        {
            genre = new Genre {Id = Guid.NewGuid(), GenreName = genreName};
            _dbContext.Genres.Add(genre);
            await _dbContext.SaveChangesAsync();
        }
        return genre;
    }

    private async Task<Publisher> FindOrCreatePublisherAsync(string publisherName)
    {
        var publisher = await _dbContext.Publishers.FirstOrDefaultAsync(p => p.PublisherName == publisherName);
        if (publisher == null)
        {
            publisher = new Publisher {Id = Guid.NewGuid(), PublisherName = publisherName};
            _dbContext.Publishers.Add(publisher);
            await _dbContext.SaveChangesAsync();
        }
        return publisher;
    }
}