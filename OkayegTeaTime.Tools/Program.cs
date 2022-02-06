using System;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Tools;

public static class Program
{
    private static readonly Regex _publisherPattern = NewRegex(@"^publish(er)?\s");
    private static readonly Regex _formatterPattern = NewRegex(@"^format(ter)?\s");
    private static readonly Regex _readmeGeneratorPattern = NewRegex(@"^readme\s");

    private static void Main(string[] args)
    {
        string argsStr = string.Join(' ', args);
        if (_publisherPattern.IsMatch(argsStr))
        {
            Publisher publisher = new(args);
            publisher.Publish();
        }
        else if (_formatterPattern.IsMatch(argsStr))
        {
        }
        else if (_readmeGeneratorPattern.IsMatch(argsStr))
        {
        }
    }

    public static Regex NewRegex(string pattern)
    {
        return new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    }
}
