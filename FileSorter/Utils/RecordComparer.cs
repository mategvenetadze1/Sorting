namespace FileSorter.Utils;

internal class RecordComparer : IComparer<Record>
{
    public int Compare(Record? x, Record? y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        var result = string.Compare(x.Text, y.Text, StringComparison.Ordinal);

        if (result == 0) 
        {
            // Compare by number if string is equal
            return x.Number.CompareTo(y.Number);
        }

        return result;
    }
}
