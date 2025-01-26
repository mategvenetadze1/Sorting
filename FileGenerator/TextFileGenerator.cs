using System.Text;

namespace FileGenerator;

public class TextFileGenerator
{
    private readonly string _filePath;
    private readonly long _size;

    public TextFileGenerator(string? filePath, long size)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
        }

        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Size must be greater than zero.");
        }

        _filePath = filePath;
        _size = size;
    }

    public void GenerateRandomLines(string[] words)
    {
        var random = new Random();

        using var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096);
        using var streamWriter = new StreamWriter(fileStream, Encoding.UTF8, 65536);

        long currentSize = 0;

        while (currentSize < _size)
        {
            var number = random.Next(1, 2000000);
            var text = words[random.Next(words.Length)];
            var line = $"{number}. {text}";

            streamWriter.WriteLine(line);
            currentSize += (line.Length + Environment.NewLine.Length);
        }
    }
}
