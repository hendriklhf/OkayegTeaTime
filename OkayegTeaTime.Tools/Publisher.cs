using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Tools;

public class Publisher
{
    private readonly string[] _args;

    private readonly Dictionary<string, Regex> _runtimes = new(new KeyValuePair<string, Regex>[]
    {
        new("win-x64", NewRegex("^win(dows)?(-?x?64)?$")),
        new("linux-arm", NewRegex("^((linux-?)?arm)|((raspberry)?pi)$")),
        new("linux-x64", NewRegex("^linux(-?x?64)?$")),
        new("osx-x64", NewRegex("^((osx)|(mac(-?os)?)(-?x64)?)$"))
    });

    public Publisher(string[] args)
    {
        _args = args;
    }

    public void Publish()
    {
        string[] runtimes = GetRuntimes();
        if (!runtimes.Any())
        {
            Console.WriteLine("The provided runtimes aren't matching any available runtime identifier.");
            return;
        }

        foreach (string r in runtimes)
        {
            Process cmd = new();
            string outputDir = $"./Build/{r}/";
            ClearDirectory(outputDir);
            cmd.StartInfo = new("dotnet", $"publish -r {r} -c Release -o {outputDir} --no-self-contained ./OkayegTeaTime/OkayegTeaTime.csproj");
            cmd.OutputDataReceived += (_, e) => Console.WriteLine(e.Data);
            cmd.ErrorDataReceived += (_, e) => Console.WriteLine(e.Data);
            cmd.Start();
            cmd.WaitForExit();
        }
    }

    private string[] GetRuntimes() => _runtimes.Where(kv => _args[1..].Any(a => kv.Value.IsMatch(a))).Select(kv => kv.Key).ToArray();

    private static void ClearDirectory(string dir)
    {
        if (!Directory.Exists(dir))
        {
            return;
        }

        Directory.Delete(dir, true);
    }
}
