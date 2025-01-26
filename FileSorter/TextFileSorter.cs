using FileSorter.Utils;
using FileSorter.Utils.Extensions;
using System.Text;

namespace FileSorter;

public class TextFileSorter
{
    private readonly string _inputFilePath;
    private readonly string _outputFilePath;

    public TextFileSorter(string? inputFilePath, string? outputFilePath)
    {
        if (string.IsNullOrEmpty(inputFilePath))
        {
            throw new ArgumentNullException(nameof(inputFilePath), "Input file path cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(outputFilePath))
        {
            throw new ArgumentNullException(nameof(outputFilePath), "Output file path cannot be null or empty.");
        }

        _inputFilePath = inputFilePath;
        _outputFilePath = outputFilePath;
    }

    public void SortTextFile(int maxChunkSize)
    {
        var tempFiles = InitializeTempFiles(_inputFilePath, maxChunkSize);

        try
        {
            MergeRuns(tempFiles, _outputFilePath);
        }
        finally
        {
            foreach (var file in tempFiles)
            {
                try { File.Delete(file); } catch { }
            }
        }
    }

    #region Private Methods
    private List<string> InitializeTempFiles(string inputFilePath, int maxChunkSize)
    {
        var tempFiles = new List<string>();
        var maxBytes = maxChunkSize * 1024L * 1024L;
        var currentMemoryUsage = 0;
        var currentChunk = new List<Record>();

        using var fileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read);
        using var streamReader = new StreamReader(fileStream, Encoding.UTF8, bufferSize: 65536);

        while (true)
        {
            var record = streamReader.ToRecord();

            if (record == null) break;

            currentChunk.Add(record);
            currentMemoryUsage += sizeof(int) + (record.Text.Length * sizeof(char)) + 32;

            if (currentMemoryUsage >= maxBytes) // Check memory limit
            {
                ProcessCurrentChunk(currentChunk, tempFiles);
                currentMemoryUsage = 0;
            }
        }

        ProcessCurrentChunk(currentChunk, tempFiles);

        return tempFiles;
    }

    private void ProcessCurrentChunk(List<Record> chunk, List<string> tempFiles)
    {
        if (chunk.Count == 0) return;

        chunk.Sort(new RecordComparer());

        var tempFile = Path.GetTempFileName();

        using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
        using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8, 65536))
        {
            foreach (var record in chunk)
            {
                streamWriter.WriteLine($"{record.Number}. {record.Text}");
            }
        }

        tempFiles.Add(tempFile);
        chunk.Clear();
    }

    private void MergeRuns(List<string> sortedFiles, string outputFilePath)
    {
        using var fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
        using var streamWriter = new StreamWriter(fileStream, Encoding.UTF8, 65536);

        var readers = new List<StreamReader>();
        var priorityQueue = new PriorityQueue<(Record record, int readerIndex), (string, int)>();

        try
        {
            // Enqueue the first record
            for (int i = 0; i < sortedFiles.Count; i++)
            {
                var reader = sortedFiles[i].ToStreamReader();
                readers.Add(reader);

                var firstRecord = reader.ToRecord();
                if (firstRecord != null)
                {
                    priorityQueue.Enqueue((firstRecord, i), (firstRecord.Text, firstRecord.Number));
                }
            }

            // Extract, write, read
            while (priorityQueue.Count > 0)
            {
                var (record, idx) = priorityQueue.Dequeue();
                streamWriter.WriteLine($"{record.Number}. {record.Text}");

                var nextRecord = readers[idx].ToRecord();
                if (nextRecord != null)
                {
                    priorityQueue.Enqueue((nextRecord, idx), (nextRecord.Text, nextRecord.Number));
                }
            }
        }
        finally
        {
            foreach (var reader in readers)
            {
                reader.Close();
            }
        }
    } 
    #endregion
}
