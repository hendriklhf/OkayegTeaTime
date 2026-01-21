global using static OkayegTeaTime.Tools.Program;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OkayegTeaTime.Tools;

internal static class Program
{
    private static Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Write("Args: ");
            args = Console.ReadLine()?.Split() ?? throw new ArgumentNullException(nameof(args));
        }

        switch (args[0])
        {
            case "build":
                Builder builder = new(args);
                return builder.BuildAsync();
            case "readme":
                ReadMeGenerator.Generate();
                break;
            case "clean":
                SolutionCleaner cleaner = new();
                cleaner.Clean();
                break;
        }

        return Task.CompletedTask;
    }

    public static Regex NewRegex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
        => new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
}
