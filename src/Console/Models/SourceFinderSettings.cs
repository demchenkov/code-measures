using Microsoft.CodeAnalysis.CSharp;

namespace Console.Models;

public static class SourceFinderSettings
{
    public static bool IsSelectionStatement(this SyntaxKind kind) => kind is >= SyntaxKind.IfStatement and <= SyntaxKind.FinallyClause;
    public static bool IsIterationStatement (this SyntaxKind kind) => kind is >= SyntaxKind.WhileStatement and <= SyntaxKind.ForEachStatement;
    public static bool IsJumpStatement(this SyntaxKind kind) => kind is >= SyntaxKind.GotoStatement and <= SyntaxKind.ThrowStatement;
    public static bool IsGeneralStatement(this SyntaxKind kind) => kind is SyntaxKind.SemicolonToken;
}