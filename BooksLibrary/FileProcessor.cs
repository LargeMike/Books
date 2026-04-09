using BooksLibrary.Models;

namespace BooksLibrary;

public class FileProcessor
{
    private readonly LineParser _lineParser;
    private readonly List<string> _fields;
    private bool _headersRead = false;
    
    public IEnumerable<string> CurrentField => _fields;

    public FileProcessor(LineParser LineParser)
    {
        _lineParser = LineParser;
        _fields = new List<string>();
    }

    public void ProcessLine(string line)
    {
        if (!_headersRead)
        {
            _fields.Clear();
            var headers = line.Split(',');
            foreach (var header in headers)
            {
                _fields.Add(header);
            }
            _headersRead = true;
            return;
        }
        
        _lineParser.ParseLine(line, _fields.ToArray());
    }
}