using BooksLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksLibrary;

public class BooksDbContext : DbContext
{
   public DbSet<Book> Books { get; set; }
   public DbSet<Author> Authors { get; set; }
   public DbSet<Publisher> Publishers { get; set; }
   public DbSet<Genre> Genres { get; set; }

   public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options)
   {
   }
}