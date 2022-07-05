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

    private const string _apiProjectPath = "./OkayegTeaTime.Api/OkayegTeaTime.Api.csproj";
    private const string _botProjectPath = "./OkayegTeaTime/OkayegTeaTime.csproj";
    private const string _commitIdSourcePath = "./.git/logs/HEAD";
    private const string _commitIdFile = "./OkayegTeaTime.Resources/LastCommit";

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

        foreach (string runtime in runtimes)
        {
            Console.WriteLine($"Starting builds for {runtime} runtime.");
            string outputDir = $"./Build/{runtime}/";
            ClearDirectory(outputDir);
            GetLastCommitId();
            BuildApi(outputDir, runtime);
            BuildBot(outputDir, runtime);
        }
    }

    private string[] GetRuntimes() => _runtimes.Where(kv => _args[1..].Any(a => kv.Value.IsMatch(a))).Select(kv => kv.Key).ToArray();

    private static void BuildApi(string outputDir, string runtime)
    {
        StartBuildProcess(outputDir, runtime, _apiProjectPath, _apiProjectPath[16..19]);
    }

    private static void BuildBot(string outputDir, string runtime)
    {
        StartBuildProcess(outputDir, runtime, _botProjectPath, _botProjectPath[2..15]);
    }

    private static void StartBuildProcess(string outputDir, string runtime, string projectPath, string projectName)
    {
        Process buildProcess = new()
        {
            StartInfo = new("dotnet", $"publish -r {runtime} -c Release -o {outputDir} --no-self-contained {projectPath}")
        };
        buildProcess.StartInfo.RedirectStandardOutput = true;
        buildProcess.ErrorDataReceived += (_, e) => Console.WriteLine(e.Data);
        Console.WriteLine($"Starting {projectName} build...");
        Console.WriteLine($"Output directory: {outputDir}");
        buildProcess.Start();
        buildProcess.WaitForExit();
        Console.WriteLine($"Finished {projectName} build!");
    }

    private static void ClearDirectory(string dir)
    {
        if (!Directory.Exists(dir))
        {
            return;
        }

        Directory.Delete(dir, true);
    }

    private static void GetLastCommitId()
    {
        string[] lines = File.ReadAllLines(_commitIdSourcePath);
        File.WriteAllText(_commitIdFile, lines[^1].Split(' ')[1][..7]);
    }
}
