using System.Text;

namespace FileSorter.Utils.Extensions;

internal static class Extensions
{
    public static Record? ToRecord(this StreamReader streamReader)
    {
        var line = streamReader.ReadLine();

        if (line == null) return null;

        int dotIndex = line.IndexOf(". ");

        if (dotIndex < 0) return null;

        if (!int.TryParse(line.AsSpan(0, dotIndex), out int number)) return null;

        var text = line[(dotIndex + 2)..];

        return new Record
        {
            Number = number,
            Text = text
        };
    }

    public static StreamReader ToStreamReader(this string path)
    {
        return new StreamReader(
            new FileStream(path, FileMode.Open, FileAccess.Read),
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            bufferSize: 65536
        );
    }
}
