using Books.Resources;
using BooksLibrary;

namespace Books.UI;

public class InputHandler
{
    public string? FilePath { get; private set; }
    private readonly UserFileProcessor  _userFileProcessor;

    public InputHandler(UserFileProcessor userFileProcessor)
    {
        _userFileProcessor = userFileProcessor ?? throw new ArgumentNullException(nameof(userFileProcessor));
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