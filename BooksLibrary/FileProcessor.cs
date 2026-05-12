namespace BooksLibrary;

public class FileProcessor
{
    private readonly LineParser _lineParser;
    private readonly List<string> _fields;
    private bool _headersRead = false;
    private readonly BookSaver _bookSaver;
    
    public FileProcessor(LineParser lineParser, BookSaver bookSaver)
    {
        _lineParser = lineParser;
        _bookSaver = bookSaver;
        _fields = new List<string>();
    }

    public async Task ProcessLineAsync(string line)
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
            return;
        }
        
        var parsedBook = _lineParser.ParseLine(line, _fields.ToArray());
        if (parsedBook != null)
            await _bookSaver.SaveAsync(parsedBook);
    }
}