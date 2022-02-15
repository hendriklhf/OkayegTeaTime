using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using HLE.Collections;

namespace OkayegTeaTime.Tools;

public class Publisher
{
    public string[] Args { get; }

    private static readonly string[] _runtimes =
    {
        "win-x64",
        "linux-arm",
        "linux-x64",
        "osx-x64"
    };

    private static readonly Dictionary<string, Regex> _regexDic = new(new KeyValuePair<string, Regex>[]
    {
        new(_runtimes[0], NewRegex("^win(dows)?(-?x?64)?$")),
        new(_runtimes[1], NewRegex("^((linux-?)?arm)|((raspberry)?pi)$")),
        new(_runtimes[2], NewRegex("^linux(-?x?64)?$")),
        new(_runtimes[3], NewRegex("^((osx)|(mac(-?os)?)(-?x64)?)$"))
    });

    public Publisher(string[] args)
    {
        Args = args;
    }

    public void Publish()
    {
        List<string> runtimes = GetRuntimes();
        if (!runtimes.Any())
        {
            Console.WriteLine($"The provided runtimes aren't matching any available runtime identifier.");
            Environment.Exit(1);
            return;
        }

        foreach (string r in runtimes)
        {
            Process cmd = new();
            cmd.StartInfo = new("dotnet", $"publish -r {r} -c Release -o ./Build/{r}/ -p:PublishSingleFile=true --self-contained true ./OkayegTeaTime/OkayegTeaTime.csproj");
            cmd.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            cmd.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);
            cmd.Start();
            cmd.WaitForExit();
        }
    }

    private List<string> GetRuntimes()
    {
        List<string> result = new();
        _regexDic.ForEach(v =>
        {
            if (Args.Any(a => v.Value.IsMatch(a)))
            {
                result.Add(v.Key);
            }
        });
        return result;
    }
}
