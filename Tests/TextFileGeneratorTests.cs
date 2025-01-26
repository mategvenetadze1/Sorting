using FileGenerator;

namespace Tests;

public class TextFileGeneratorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ThrowsIfFilePathIsNullOrEmpty(string? filePath)
    {
        // Arrange
        var size = 1024L;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TextFileGenerator(filePath, size));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void ThrowsIfSizeIsNotValid(long size)
    {
        // Arrange
        var filePath = "test.txt";

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new TextFileGenerator(filePath, size));
    }

    [Fact]
    public void GenerateRandomLines_CreatesNonEmptyFile()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();

        try
        {
            var size = 200L;
            var generator = new TextFileGenerator(tempFile, size);
            var words = new string[] { "Test", "Apple", "Banana" };

            // Act
            generator.GenerateRandomLines(words);

            // Assert
            Assert.True(File.Exists(tempFile), "File should be created.");

            var fileInfo = new FileInfo(tempFile);

            Assert.True(fileInfo.Length > 0, "File size should be greater than zero.");
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}
