global using static OkayegTeaTime.Tools.Program;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Tools;

public static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Write("Args: ");
            args = Console.ReadLine()?.Split() ?? throw new ArgumentNullException(nameof(args));
        }

        switch (args[0])
        {
            case "build":
            {
                Builder builder = new(args);
                builder.Build();
                break;
            }
            case "readme":
            {
                ReadMeGenerator generator = new();
                generator.Generate();
                break;
            }
            case "clean":
            {
                SolutionCleaner cleaner = new();
                cleaner.Clean();
                break;
            }
        }
    }

    public static Regex NewRegex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
    {
        return new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    }
}
