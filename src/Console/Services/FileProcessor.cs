using System.Text.RegularExpressions;
using Console.Extensions;
using Console.Models;

namespace Console.Services;

public class FileProcessor
{
    private readonly string _oneLineCommentKey = "//";
    private readonly string _multiLineCommentStartKey = "/*";
    private readonly string _multiLineCommentEndKey = "*/";
    private readonly SourceFinderSettings _sourceFinderSettings;


    public FileProcessor(
        string? oneLineCommentKey = null,
        string? multiLineCommentStartKey = null,
        string? multiLineCommentEndKey = null,
        SourceFinderSettings? sourceFinderSettings = null)
    {
        _oneLineCommentKey = oneLineCommentKey ?? _oneLineCommentKey;
        _multiLineCommentStartKey = multiLineCommentStartKey ?? _multiLineCommentStartKey;
        _multiLineCommentEndKey = multiLineCommentEndKey ?? _multiLineCommentEndKey;
        _sourceFinderSettings = sourceFinderSettings ?? new SourceFinderSettings();
    }
    
    public CodeStatistic Process(string fileContent)
    {
        var lines = fileContent.Split(Environment.NewLine).Select(x => x.Trim()).ToList();

        var result = new CodeStatistic
        {
            Lines = lines.Count,
            EmptyLines = lines.Count(string.IsNullOrEmpty),
            CommentLines = CountCommentedLines(lines),
            SourceLines = CountSourceLines(lines)
        };

        return result;
    }

    private int CountSourceLines(List<string> lines)
    {
        var fullSource = string.Join(Environment.NewLine, lines);
        var sourceWithoutComments = Regex.Replace(fullSource, _sourceFinderSettings.RemoveCommentsPattern, "$1");

        int FindUsageCount(string pattern) => Regex.Matches(sourceWithoutComments!, pattern, RegexOptions.Multiline).Count;

        var selectionStatementsCount = FindUsageCount(_sourceFinderSettings.SelectionStatementPattern) ;
        var iterationStatementsCount = FindUsageCount(_sourceFinderSettings.IterationStatementPattern);
        var jumpStatementsCount = FindUsageCount(_sourceFinderSettings.JumpStatementPattern);

        var emptyStatementsCount = FindUsageCount(_sourceFinderSettings.EmptyStatementPattern);
        
        var expressionStatementsCount = FindUsageCount(_sourceFinderSettings.FunctionCallStatementPattern)
                                        + FindUsageCount(_sourceFinderSettings.AssigmentStatementPattern)
                                        + emptyStatementsCount;

        var generalStatementsCount = FindUsageCount(_sourceFinderSettings.GeneralStatementPattern);
        var blockDelimitersCount = emptyStatementsCount - selectionStatementsCount;

        return selectionStatementsCount
               + iterationStatementsCount
               + jumpStatementsCount
               + expressionStatementsCount
               + generalStatementsCount
               + blockDelimitersCount;
    }


    private int CountCommentedLines(List<string> codeLines) 
    {
        var isInCommentState = false;

        return codeLines
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Aggregate(0, (acc, cur) => 
        {
            if (isInCommentState) 
            {
                isInCommentState = !cur.Contains(_multiLineCommentEndKey);
                return acc + 1;
            }

            var oneLineCommentIndex = cur.IndexOfOrDefault(_oneLineCommentKey);
            var multiLineCommentStartIndex = cur.IndexOfOrDefault(_multiLineCommentStartKey);

            if (oneLineCommentIndex == multiLineCommentStartIndex) // line doesn't contain a comment
                return acc;

            isInCommentState = multiLineCommentStartIndex < oneLineCommentIndex;
            return acc + 1;
        });
    }
}