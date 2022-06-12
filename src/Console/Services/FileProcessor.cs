using System.Text.RegularExpressions;
using Console.Extensions;
using Console.Models;
using Microsoft.CodeAnalysis.CSharp;

namespace Console.Services;

public class FileProcessor
{
    private readonly string _oneLineCommentKey = "//";
    private readonly string _multiLineCommentStartKey = "/*";
    private readonly string _multiLineCommentEndKey = "*/";


    public FileProcessor(
        string? oneLineCommentKey = null,
        string? multiLineCommentStartKey = null,
        string? multiLineCommentEndKey = null)
    {
        _oneLineCommentKey = oneLineCommentKey ?? _oneLineCommentKey;
        _multiLineCommentStartKey = multiLineCommentStartKey ?? _multiLineCommentStartKey;
        _multiLineCommentEndKey = multiLineCommentEndKey ?? _multiLineCommentEndKey;
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
        
        var root = CSharpSyntaxTree.ParseText(fullSource).GetRoot();
        var nodes = root.DescendantNodes().ToArray();
        


        var selectionStatementsCount = nodes.Count(x => x.Kind().IsSelectionStatement());
        var iterationStatementsCount = nodes.Count(x => x.Kind().IsIterationStatement());
        var jumpStatementsCount = nodes.Count(x => x.Kind().IsJumpStatement());
        var expressionStatementsCount = nodes.Count(x => x.Kind() == SyntaxKind.IsExpression);
        var generalStatementsCount = nodes.Count(x => x.Kind().IsGeneralStatement());
        var blockDelimitersCount = nodes.Count(x => x.Kind() == SyntaxKind.Block);
        
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