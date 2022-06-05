// See https://aka.ms/new-console-template for more information

using Console.Models;
using Console.Services;

var path = args.FirstOrDefault() ?? $"{Environment.CurrentDirectory}/../../../";
var extension = args.Skip(1).FirstOrDefault() ?? "cs";

var processor = new FileProcessor();
var filePaths = Directory.GetFiles(path, $"*.{extension}", SearchOption.AllDirectories);

var statistic = filePaths.Aggregate<string, CodeStatistic?>(null, (acc, path) =>
{
    var res = processor.Process(File.ReadAllText(path));
    res.Lines += acc?.Lines ?? 0;
    res.CommentLines += acc?.CommentLines ?? 0;
    res.EmptyLines += acc?.EmptyLines ?? 0;
    res.SourceLines += acc?.SourceLines ?? 0;

    return res;
});

System.Console.WriteLine($"Total lines: {statistic.Lines}");
System.Console.WriteLine($"Empty lines: {statistic.EmptyLines}");
System.Console.WriteLine($"Physical lines: {statistic.GetPhysicalLines()}");
System.Console.WriteLine($"Source lines: {statistic.SourceLines}");
System.Console.WriteLine($"Comment lines: {statistic.CommentLines}");
System.Console.WriteLine($"Commenting level: {statistic.CommentingLevel}");

