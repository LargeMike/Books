using System.Globalization;
using BooksLibrary;
using CsvHelper;

namespace BooksLibraryTests;

[TestClass]
public class LineParserTests
{
    private const string Header = "Title,Author,Genre,Publisher,Pages,ReleaseDate";

    private static ParsedBook Parse(string dataLine)
    {
        using var reader = new StringReader($"{Header}\n{dataLine}");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<LineParser>();
        return csv.GetRecords<ParsedBook>().Single();
    }

    [TestMethod]
    public void Parse_ValidLine_MapsAllProperties()
    {
        var result = Parse("Clean Code,Robert Martin,Programming,O'Reilly,431,2008-08-01");

        Assert.AreEqual("Clean Code", result.Title);
        Assert.AreEqual("Robert Martin", result.Author);
        Assert.AreEqual("Programming", result.Genre);
        Assert.AreEqual("O'Reilly", result.Publisher);
        Assert.AreEqual(431, result.Pages);
        Assert.AreEqual(new DateTime(2008, 8, 1), result.ReleaseDate);
    }

    [TestMethod]
    public void Parse_QuotedFieldWithComma_KeptAsSingleField()
    {
        var result = Parse("\"Hello, World\",Some Author,Genre,Publisher,100,2020-01-15");

        Assert.AreEqual("Hello, World", result.Title);
    }

    [TestMethod]
    public void Parse_EmptyField_MapsToNullOrEmpty()
    {
        var result = Parse("Some Book,Some Author,,Publisher,100,2020-01-15");

        Assert.IsTrue(string.IsNullOrEmpty(result.Genre));
    }

    [TestMethod]
    public void Parse_InvalidDate_SetsNotParsedDate()
    {
        var result = Parse("Some Book,Some Author,Genre,Publisher,100,not-a-date");

        Assert.IsNull(result.ReleaseDate);
        Assert.AreEqual("not-a-date", result.NotParsedDate);
    }

    [TestMethod]
    public void Parse_ValidDate_ReleaseDateSetAndNotParsedDateIsNull()
    {
        var result = Parse("Some Book,Some Author,Genre,Publisher,100,2020-01-15");

        Assert.IsNotNull(result.ReleaseDate);
        Assert.IsNull(result.NotParsedDate);
    }
}
