using FileGenerator;
using FileSorter;

const string INPUT_FILE_PATH = "words.txt";
const string OUTPUT_FILE_PATH = "words-sorted.txt";

const int FILE_SIZE = 100_000_000; // MB
const int MAX_CHUNK_SIZE = 256; // MB

var words = new string[]
{
    "Apple",
    "Banana is yellow",
    "Cherry is the best",
    "Something something something",
    "Red",
    "White",
    "Mango",
    "Black"
};

var fileGenerator = new TextFileGenerator(INPUT_FILE_PATH, FILE_SIZE);
var fileSorter = new TextFileSorter(INPUT_FILE_PATH, OUTPUT_FILE_PATH);

fileGenerator.GenerateRandomLines(words);
fileSorter.SortTextFile(MAX_CHUNK_SIZE);

