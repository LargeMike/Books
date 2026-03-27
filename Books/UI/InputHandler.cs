using Books.Resources;
using BooksLibrary;

namespace Books.UI;

public class InputHandler
{
    public string? FilePath { get; private set; }
    private readonly FileProcessor  _userFileProcessor;

    public InputHandler(FileProcessor FileProcessor)
    {
        _userFileProcessor = FileProcessor ?? throw new ArgumentNullException(nameof(FileProcessor));
        FilePath = null;
        GetFilePath();
    }

    public void GetFilePath()
    {
        Console.Write(Messages.InputPrompt);
        var filePath = Console.ReadLine();

        if (!File.Exists(filePath))
        {
            Console.Write(Messages.FileNotFound);
            return;
        }
        
        GetFileContent(filePath);
    }
    
    private void GetFileContent(string filePath)
    {
        var lines = File.ReadLines(filePath);

        try
        {
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    _userFileProcessor.ProcessLine(line);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine(Messages.AccessDenied);
        }
    }
}