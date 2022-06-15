using System;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Tools;

public static class Program
{
    private static void Main(string[] args)
    {
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
            case "sync":
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
        }
    }

    public static Regex NewRegex(string pattern)
    {
        return new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    }
}
