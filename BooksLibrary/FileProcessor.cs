using BooksLibrary.Models;

namespace BooksLibrary;

public class FileProcessor
{
    private readonly LineParser _lineParser;
    private readonly List<string> _fields;
    
    public IEnumerable<string> CurrentField => _fields;

    public FileProcessor(LineParser LineParser)
    {
        _lineParser = LineParser;
        _fields = new List<string>();
    }

    public void ProcessLine(string filePath)
    {
        _fields.Clear();
        _lineParser.ParseLine(filePath);
        
    }
}