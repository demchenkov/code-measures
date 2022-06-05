namespace Console.Extensions;

public static class StringExtensions
{
    public static int IndexOfOrDefault(this string str, string substr, int @default = int.MaxValue)
    {
        var index = str.IndexOf(substr, StringComparison.Ordinal);
        return index == -1 ? @default : index;
    }
}