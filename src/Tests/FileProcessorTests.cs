using Console.Services;

namespace Tests;

public class FileProcessorTests
{
    private readonly FileProcessor _processor = new FileProcessor();
    
    [Theory]
    [InlineData("//one comment line", 1)]
    [InlineData("//one comment line\r\n//second comment", 2)]
    [InlineData("/*one comment line\r\n second comment\r\n third comment */", 3)]
    public void ShouldContainsOnlyComments(string text, int commentCount)
    {
        var stats = _processor.Process(text);
        
        Assert.Equal(commentCount, stats.Lines);
        Assert.Equal(commentCount, stats.CommentLines);
        Assert.Equal(0, stats.EmptyLines);
        Assert.Equal(0, stats.SourceLines);
        Assert.Equal(1.0, stats.CommentingLevel);
    }
    
    [Theory]
    [InlineData("", 1)]
    [InlineData("\r\n", 2)]
    [InlineData("\r\n\r\n", 3)]
    public void ShouldContainsOnlyEmptyLines(string text, int count)
    {
        var stats = _processor.Process(text);
        
        Assert.Equal(count, stats.Lines);
        Assert.Equal(count, stats.EmptyLines);
        Assert.Equal(0, stats.CommentLines);
        Assert.Equal(0, stats.SourceLines);
        Assert.Equal(0.0, stats.CommentingLevel);
    }
    
    [Theory]
    [InlineData("if (true) return 0;", 1, 2)]
    [InlineData("while (true) return 0;", 1, 2)]
    [InlineData("while (true) \r\nreturn 0;", 2, 2)]
    public void ShouldContainsCode(string text, int lines, int count)
    {
        var stats = _processor.Process(text);
        
        Assert.Equal(lines, stats.Lines);
        Assert.Equal(0, stats.EmptyLines);
        Assert.Equal(0, stats.CommentLines);
        Assert.Equal(count, stats.SourceLines);
        Assert.Equal(0.0, stats.CommentingLevel);
    }
}