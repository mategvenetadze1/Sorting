using FileSorter;

namespace Tests;

public class TextFileSorterTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ThrowsIfInputPathNullOrEmpty(string? inputPath)
    {
        // Arrange
        var outputPath = "test-sorted.txt";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TextFileSorter(inputPath, outputPath));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ThrowsIfOutputPathNullOrEmpty(string? outputPath)
    {
        // Arrange
        var inputPath = "test.txt";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TextFileSorter(inputPath, outputPath));
    }

    [Fact]
    public void SortTextFile_SortsCorrectly()
    {
        // Arrange
        var inputFile = Path.GetTempFileName();
        var outputFile = Path.GetTempFileName();

        try
        {
            File.WriteAllLines(inputFile,
            [
                "10. Apple",
                "2. Banana",
                "2. Apple",
                "7. Mango",
                "2. Mango"
            ]);

            var sorter = new TextFileSorter(inputFile, outputFile);

            // Act
            sorter.SortTextFile(maxChunkSize: 1);

            // Assert
            var sortedLines = File
                .ReadAllLines(outputFile)
                .ToList();

            //  "2. Apple"
            //  "10. Apple"
            //  "2. Banana"
            //  "2. Mango"
            //  "7. Mango"

            Assert.Equal(5, sortedLines.Count);

            Assert.Equal("2. Apple", sortedLines[0]);
            Assert.Equal("10. Apple", sortedLines[1]);
            Assert.Equal("2. Banana", sortedLines[2]);
            Assert.Equal("2. Mango", sortedLines[3]);
            Assert.Equal("7. Mango", sortedLines[4]);
        }
        finally
        {
            if (File.Exists(inputFile))
            {
                File.Delete(inputFile);
            }

            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }
        }
    }
}
