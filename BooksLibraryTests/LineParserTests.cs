using BooksLibrary;

namespace BooksLibraryTests;

[TestClass]
public class LineParserTests
{
    private readonly LineParser _parser = new();

    private static readonly string[] DefaultHeaders =
        ["Title", "Author", "Genre", "Publisher", "Pages", "ReleaseDate"];

    [TestMethod]
    public void SplitCsvLine_SimpleLine_ReturnsCorrectFields()
    {
        var result = _parser.SplitCsvLine("a,b,c");

        CollectionAssert.AreEqual(new[] { "a", "b", "c" }, result);
    }

    [TestMethod]
    public void SplitCsvLine_QuotedFieldWithComma_ReturnsFieldAsOne()
    {
        var result = _parser.SplitCsvLine("\"Hello, World\",b");

        CollectionAssert.AreEqual(new[] { "Hello, World", "b" }, result);
    }

    [TestMethod]
    public void SplitCsvLine_EmptyField_ReturnsEmptyString()
    {
        var result = _parser.SplitCsvLine("a,,c");

        CollectionAssert.AreEqual(new[] { "a", "", "c" }, result);
    }

    [TestMethod]
    public void ParseLine_ValidLine_MapsAllProperties()
    {
        var line = "Clean Code,Robert Martin,Programming,O'Reilly,431,2008-08-01";

        var result = _parser.ParseLine(line, DefaultHeaders);

        Assert.AreEqual("Clean Code", result.Title);
        Assert.AreEqual("Robert Martin", result.Author);
        Assert.AreEqual("Programming", result.Genre);
        Assert.AreEqual("O'Reilly", result.Publisher);
        Assert.AreEqual(431, result.Pages);
        Assert.AreEqual(new DateTime(2008, 8, 1), result.ReleaseDate);
    }

    [TestMethod]
    public void ParseLine_InvalidDate_SetsNotParsedDate()
    {
        var line = "Some Book,Some Author,Genre,Publisher,100,not-a-date";

        var result = _parser.ParseLine(line, DefaultHeaders);

        Assert.IsNull(result.ReleaseDate);
        Assert.AreEqual("not-a-date", result.NotParsedDate);
    }

    [TestMethod]
    public void ParseLine_ValidDate_ReleaseDateSetAndNotParsedDateIsNull()
    {
        var line = "Some Book,Some Author,Genre,Publisher,100,2020-01-15";

        var result = _parser.ParseLine(line, DefaultHeaders);

        Assert.IsNotNull(result.ReleaseDate);
        Assert.IsNull(result.NotParsedDate);
    }
}
