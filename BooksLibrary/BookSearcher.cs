using BooksLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksLibrary;

public class BookSearcher
{
    private readonly BooksDbContext _dbContext;

    public BookSearcher(BooksDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Book>> SearchAsync(BookFilter filter)
    {
        IQueryable<Book> query = _dbContext.Books
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .Include(b => b.Publisher);

        if (filter.Title != null)
            query = query.Where(b => b.Title.Contains(filter.Title));
        if (filter.Author != null)
            query = query.Where(b => b.Author.AuthorName == filter.Author);
        if (filter.Genre != null)
            query = query.Where(b => b.Genre.GenreName == filter.Genre);
        if (filter.Publisher != null)
            query = query.Where(b => b.Publisher.PublisherName == filter.Publisher);
        if (filter.MoreThanPages != null)
            query = query.Where(b => b.NumberOfPages > filter.MoreThanPages);
        if (filter.LessThanPages != null)
            query = query.Where(b => b.NumberOfPages < filter.LessThanPages);
        if (filter.PublishedBefore != null)
            query = query.Where(b => b.DatePublished < filter.PublishedBefore);
        if (filter.PublishedAfter != null)
            query = query.Where(b => b.DatePublished > filter.PublishedAfter);

        return await query.ToListAsync();
    }
}