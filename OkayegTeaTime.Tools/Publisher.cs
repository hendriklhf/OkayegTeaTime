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
            Console.WriteLine($"Starting builds for {r} runtime.");
            string outputDir = $"./Build/{r}/";
            ClearDirectory(outputDir);

            Process apiBuild = new()
            {
                StartInfo = new("dotnet", $"publish -r {r} -c Release -o {outputDir} --no-self-contained ./OkayegTeaTime.Api/OkayegTeaTime.Api.csproj")
            };
            apiBuild.StartInfo.RedirectStandardOutput = true;
            apiBuild.ErrorDataReceived += (_, e) => Console.WriteLine(e.Data);
            Console.WriteLine("Starting API build...");
            Console.WriteLine($"Output directory: {outputDir}");
            apiBuild.Start();
            apiBuild.WaitForExit();
            Console.WriteLine("Finished API build!");

            Process botBuild = new()
            {
                StartInfo = new("dotnet", $"publish -r {r} -c Release -o {outputDir} --no-self-contained ./OkayegTeaTime/OkayegTeaTime.csproj")
            };
            botBuild.StartInfo.RedirectStandardOutput = true;
            botBuild.ErrorDataReceived += (_, e) => Console.WriteLine(e.Data);
            Console.WriteLine("Starting OkayegTeaTime build...");
            Console.WriteLine($"Output directory: {outputDir}");
            botBuild.Start();
            botBuild.WaitForExit();
            Console.WriteLine("Finished OkayegTeaTime build!");
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
