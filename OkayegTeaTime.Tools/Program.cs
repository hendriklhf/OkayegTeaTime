using System;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Tools;

public static class Program
{
    private static readonly Regex _publisherPattern = NewRegex(@"^publish(er)?");
    private static readonly Regex _readmeGeneratorPattern = NewRegex(@"^readme");
    private static readonly Regex _syncerDownloadPattern = NewRegex(@"^(resource)?sync(er)?\sdown(load)?");
    private static readonly Regex _syncerUploadPattern = NewRegex(@"^(resource)?sync(er)?\sup(load)?");

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
        else if (_syncerDownloadPattern.IsMatch(argsStr))
        {
            ResourceSyncer syncer = new();
            syncer.Download();
        }
        else if (_syncerUploadPattern.IsMatch(argsStr))
        {
            ResourceSyncer syncer = new();
            syncer.Upload();
        }
    }

    public static Regex NewRegex(string pattern)
    {
        return new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    }
}
