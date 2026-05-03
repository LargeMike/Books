using BooksLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksLibrary;

public class BookSaver
{
    private readonly BooksDbContext _dbContext;

    public BookSaver(BooksDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveAsync(ParsedBook parsedBook)
    {
        if (parsedBook.ReleaseDate == null)
        {
            Console.WriteLine($"Skipping '{parsedBook.Title}' because date is not parsed");
            return;
        }
        
        var author = await FindOrCreateAuthorAsync(parsedBook.Author);
        var genre = await FindOrCreateGenreAsync(parsedBook.Genre);
        var publisher = await FindOrCreatePublisherAsync(parsedBook.Publisher);

        var isDuplicate = await _dbContext.Books.AnyAsync(b =>
            b.Title == parsedBook.Title &&
            b.AuthorId == author.Id &&
            b.PublisherId == publisher.Id &&
            b.DatePublished == parsedBook.ReleaseDate);

        if (isDuplicate)
        {
            Console.WriteLine($"Skipping '{parsedBook.Title}' because it is duplicated");
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
            DatePublished = parsedBook.ReleaseDate.Value,
        };
        
        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();

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