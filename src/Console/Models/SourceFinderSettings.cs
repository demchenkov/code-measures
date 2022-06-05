namespace Console.Models;

public class SourceFinderSettings
{
    public string RemoveCommentsPattern { get; set; } = @"(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|//.*|/\*(?s:.*?)\*/";
    
    public string SelectionStatementPattern { get; set; } = @"if|else if|else|\?|try|catch|switch";
    public string IterationStatementPattern { get; set; } = @"do[\s\S]*?while";
    public string JumpStatementPattern { get; set; } = @"return|break|goto|exit|continue|throw";
    public string FunctionCallStatementPattern { get; set; } = @"\w+\([\s\S]*?\)";
    public string AssigmentStatementPattern { get; set; } = @"[^=]=[^=>]";
    public string EmptyStatementPattern { get; set; } = @"\{\s*\}";
    public string GeneralStatementPattern { get; set; } = @";";
}