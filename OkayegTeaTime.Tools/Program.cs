using System;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Tools;

public static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            return;
        }

        switch (args[0])
        {
            case "publish":
            {
                Publisher publisher = new(args);
                publisher.Publish();
                break;
            }
            case "readme":
            {
                ReadMeGenerator generator = new();
                generator.Generate();
                break;
            }
            case "sync" when args.Length > 1:
            {
                ResourceSyncer syncer = new();
                switch (args[1])
                {
                    case "up":
                        syncer.Upload();
                        break;
                    case "down":
                        syncer.Download();
                        break;
                }

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

    public static Regex NewRegex(string pattern)
    {
        return new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    }
}
