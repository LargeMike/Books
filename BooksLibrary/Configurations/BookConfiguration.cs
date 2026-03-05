using BooksLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BooksLibrary.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
       builder.HasKey(book => book.Id);

       builder.
           HasOne(book => book.Author)
           .WithMany(author => author.Books);
       
       builder.
           HasOne(book => book.Genre)
           .WithMany(genre => genre.Books);
       
       builder.
           HasMany(book => book.Publishers)
           .WithMany(publishers => publishers.Books);
           
    }
}