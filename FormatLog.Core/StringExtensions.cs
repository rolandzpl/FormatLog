namespace Lithium.FormatLog;

public static class StringExtensions
{
    public static int LineCount(this string s)
    {
        int result = 0;
        var reader = new StringReader(s);
        while (reader.ReadLine() != null)
        {
            ++result;
        }
        return result;
    }
}
