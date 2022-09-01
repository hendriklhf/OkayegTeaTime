using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HLE.Collections;

namespace OkayegTeaTime.Tools;

public class Builder
{
    private readonly string[] _args;
    private readonly bool _selfContained;

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

    public Builder(string[] args)
    {
        _args = args;
        _selfContained = GetSelfContained();
    }

    public void Build()
    {
        string[] runtimes = GetRuntimes();
        if (!runtimes.Any())
        {
            Console.WriteLine("The provided runtimes aren't matching any available runtime identifier.");
            Console.WriteLine($"Available runtime identifiers: {_runtimes.Keys.JoinToString(", ")}");
            return;
        }

        foreach (string runtime in runtimes)
        {
            string outputDir = $"./Build/{runtime}/";
            ClearDirectory(outputDir);
            GetLastCommitId();
            Console.WriteLine($"Starting builds for {runtime} runtime.");
            BuildApi(outputDir, runtime);
            BuildBot(outputDir, runtime);
        }
    }

    private string[] GetRuntimes() => _runtimes.Where(kv => _args[1..].Any(a => kv.Value.IsMatch(a))).Select(kv => kv.Key).ToArray();

    private void BuildApi(string outputDir, string runtime)
    {
        StartBuildProcess(outputDir, runtime, _apiProjectPath, _apiProjectPath[16..19]);
    }

    private void BuildBot(string outputDir, string runtime)
    {
        StartBuildProcess(outputDir, runtime, _botProjectPath, _botProjectPath[2..15]);
    }

    private void StartBuildProcess(string outputDir, string runtime, string projectPath, string projectName)
    {
        Process buildProcess = new()
        {
            StartInfo = new("dotnet", $"publish -r {runtime} -c Release -o {outputDir} --{(_selfContained ? string.Empty : "no-")}self-contained {projectPath}")
        };
        buildProcess.StartInfo.RedirectStandardOutput = true;
        Console.WriteLine($"Starting {projectName} build...");
        Console.WriteLine($"Output directory: {outputDir}");
        buildProcess.Start();
        buildProcess.WaitForExit();
        Console.WriteLine($"Finished {projectName} build!");
    }

    private static void ClearDirectory(string dir)
    {
        Console.WriteLine($"Clearing directory: {dir}");
        if (!Directory.Exists(dir))
        {
            return;
        }

        Directory.Delete(dir, true);
    }

    private static void GetLastCommitId()
    {
        Console.WriteLine("Retrieving last commit id...");
        string[] lines = File.ReadAllLines(_commitIdSourcePath);
        string commitId = lines[^1].Split(' ')[1][..7];
        Console.WriteLine($"Last commit: {commitId}");
        File.WriteAllText(_commitIdFile, commitId);
    }

    private bool GetSelfContained() => _args.Contains("--self-contained");
}
