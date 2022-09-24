using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HLE.Collections;
using OkayegTeaTime.Files;

namespace OkayegTeaTime.Tools;

public sealed class Builder
{
    private readonly string[] _args;
    private readonly bool _selfContained;

    private readonly Dictionary<Runtime, Regex> _runtimes = new[]
    {
        (Runtime.Windows64Bit, NewRegex("^win(dows)?(-?x?64)?$")),
        (Runtime.LinuxArm, NewRegex("^((linux-?)?arm)|((raspberry)?pi)$")),
        (Runtime.Linux64Bit, NewRegex("^linux(-?x?64)?$")),
        (Runtime.MacOs64Bit, NewRegex("^((osx)|(mac(-?os)?)(-?x64)?)$"))
    }.ToDictionary();

    private const string _apiProjectPath = "./OkayegTeaTime.Api/OkayegTeaTime.Api.csproj";
    private const string _botProjectPath = "./OkayegTeaTime/OkayegTeaTime.csproj";
    private const string _commitIdSourcePath = "./.git/logs/HEAD";
    private const string _commitIdFile = "./OkayegTeaTime.Resources/LastCommit";
    private const string _codeFilesFile = "./OkayegTeaTime.Resources/CodeFiles";

    public Builder(string[] args)
    {
        _args = args;
        _selfContained = GetSelfContained();
    }

    public void Build()
    {
        Runtime[] runtimes = GetRuntimes();
        if (runtimes.Length == 0)
        {
            Console.WriteLine("The provided runtimes aren't matching any available runtime identifier.");
            Console.WriteLine($"Available runtime identifiers: {_runtimes.Keys.Select(r => r.Identifier).JoinToString(", ")}");
            return;
        }

        foreach (Runtime runtime in runtimes)
        {
            string outputDir = $"./Build/{runtime.Identifier}/";
            DeleteDirectory(outputDir);
            CreateLastCommitFile();
            CreateCodeFilesFile();
            Console.WriteLine($"Starting builds for {runtime.Name} runtime.");
            BuildApi(outputDir, runtime);
            BuildBot(outputDir, runtime);
        }
    }

    private Runtime[] GetRuntimes()
    {
        string[] args = _args[1..];
        return _runtimes.Where(kv => args.Any(a => kv.Value.IsMatch(a))).Select(kv => kv.Key).ToArray();
    }

    private void BuildApi(string outputDir, Runtime runtime)
    {
        StartBuildProcess(outputDir, runtime, _apiProjectPath, _apiProjectPath[16..19]);
    }

    private void BuildBot(string outputDir, Runtime runtime)
    {
        StartBuildProcess(outputDir, runtime, _botProjectPath, _botProjectPath[2..15]);
    }

    private void StartBuildProcess(string outputDir, Runtime runtime, string projectPath, string projectName)
    {
        Process buildProcess = new()
        {
            StartInfo = new("dotnet", $"publish -r {runtime.Identifier} -c Release -o {outputDir} --{(_selfContained ? string.Empty : "no-")}self-contained {projectPath}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        Console.WriteLine($"Starting {projectName} build");
        Console.WriteLine($"Output directory: \"{outputDir}\"");
        buildProcess.Start();
        buildProcess.WaitForExit();
        if (buildProcess.ExitCode > 0)
        {
            PrintError($"Build for {projectName} failed!");
        }
        else
        {
            Console.WriteLine($"Finished {projectName} build!");
        }
    }

    private static void DeleteDirectory(string dir)
    {
        Console.WriteLine($"Deleting directory: \"{dir}\"");
        if (!Directory.Exists(dir))
        {
            return;
        }

        Directory.Delete(dir, true);
    }

    private static void CreateLastCommitFile()
    {
        Console.WriteLine("Retrieving last commit id");
        string[] lines = File.ReadAllLines(_commitIdSourcePath);
        string commitId = lines[^1].Split(' ')[1][..7];
        Console.WriteLine($"Last commit: {commitId}");
        File.WriteAllText(_commitIdFile, commitId);
        Console.WriteLine("Created \"LastCommit\" file");
    }

    private static void CreateCodeFilesFile()
    {
        Console.WriteLine("Searching for .cs files");
        Regex fileRegex = new($@"^\.[\\/]{AppSettings.AssemblyName.Split('.')[0]}(\.\w+)?[\\/](?!((bin)|(obj)[\\/])).*\.cs$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        string[] files = Directory.GetFiles(".", "*", SearchOption.AllDirectories).Where(f => fileRegex.IsMatch(f)).Select(f => f[2..].Replace('\\', '/')).Order().ToArray();
        Console.WriteLine($"Found {files.Length} .cs files");
        File.WriteAllLines(_codeFilesFile, files);
        Console.WriteLine("Created \"CodeFiles\" file");
    }

    private bool GetSelfContained()
    {
        return _args.Contains("--self-contained");
    }

    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        try
        {
            Console.WriteLine(message);
        }
        finally
        {
            Console.ForegroundColor = default;
        }
    }
}
