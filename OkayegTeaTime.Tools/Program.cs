using System;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Tools;

public static class Program
{
    private static readonly Regex _publisherPattern = NewRegex(@"^publish(er)?");
    private static readonly Regex _readmeGeneratorPattern = NewRegex(@"^readme");

    private static void Main(string[] args)
    {
        string argsStr = string.Join(' ', args);
        if (_publisherPattern.IsMatch(argsStr))
        {
            Publisher publisher = new(args);
            publisher.Publish();
        }
        else if (_readmeGeneratorPattern.IsMatch(argsStr))
        {
            ReadMeGenerator generator = new();
            generator.GenerateReadMe();
        }
    }

    public static Regex NewRegex(string pattern)
    {
        return new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    }
}
