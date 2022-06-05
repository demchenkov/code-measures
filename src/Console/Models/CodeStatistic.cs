namespace Console.Models;

public class CodeStatistic
{
    public int Lines { get; set; }
    public int EmptyLines { get; set; }
    public int SourceLines { get; set; }
    public int CommentLines { get; set; }

    public double CommentingLevel => (double)CommentLines / Lines;

    public int GetPhysicalLines(double emptyLinesMultiplier = 0.25)
    {
        return Lines - (int)Math.Floor(EmptyLines * (1 - emptyLinesMultiplier));
    }
}