namespace BooksLibrary;

public class FileProcessor
{
    private readonly LineParser _lineParser;
    private readonly List<string> _fields;
    private bool _headersRead = false;
    
    public FileProcessor(LineParser LineParser)
    {
        _lineParser = LineParser;
        _fields = new List<string>();
    }

    public ParsedBook? ProcessLine(string line)
    {
        if (!_headersRead)
        {
            _fields.Clear();
            var headers = _lineParser.SplitCsvLine(line);
            foreach (var header in headers)
            {
                _fields.Add(header);
            }
            _headersRead = true;
            return null;
        }
        
        return _lineParser.ParseLine(line, _fields.ToArray());
    }
}