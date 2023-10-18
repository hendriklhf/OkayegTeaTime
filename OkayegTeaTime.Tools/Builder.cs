using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HLE.Collections;
using OkayegTeaTime.Settings;

namespace OkayegTeaTime.Tools;

public sealed class Builder(string[] args)
{
    private readonly string[] _args = args;

    private readonly FrozenDictionary<Runtime, Regex> _runtimes = new Dictionary<Runtime, Regex>(new KeyValuePair<Runtime, Regex>[]
    {
        new(Runtime.Windows64Bit, NewRegex("^win(dows)?(-?x?64)?$")),
        new(Runtime.LinuxArm, NewRegex("^((linux-?)?arm(64)?)|((raspberry-?)?pi)$")),
        new(Runtime.Linux64Bit, NewRegex("^linux(-?x?64)?$")),
        new(Runtime.MacOs64Bit, NewRegex("^((osx)|(mac(-?os)?)(-?x64)?)$"))
    }).ToFrozenDictionary();

    private const string _botProjectPath = "./OkayegTeaTime/OkayegTeaTime.csproj";
    private const string _commitIdSourcePath = "./.git/logs/HEAD";
    private const string _commitIdFile = "./OkayegTeaTime.Resources/LastCommit";
    private const string _codeFilesFile = "./OkayegTeaTime.Resources/CodeFiles";

    public void Build()
    {
        Runtime[] runtimes = GetRuntimes();
        if (runtimes.Length == 0)
        {
            Console.WriteLine("The provided runtimes aren't matching any available runtime identifier.");
            Console.WriteLine($"Available runtime identifiers: {_runtimes.Keys.Select(static r => r.Identifier).JoinToString(", ")}");
            return;
        }

        foreach (Runtime runtime in runtimes)
        {
            string outputDirectory = $"./Build/{runtime.Identifier}/";
            DeleteDirectory(outputDirectory);
            CreateLastCommitFile();
            CreateCodeFilesFile();
            Console.WriteLine($"Starting builds for {runtime.Name} runtime.");
            BuildBot(outputDirectory, runtime);
        }
    }

    private Runtime[] GetRuntimes()
    {
        string[] args = _args[1..];
        return _runtimes.Where(kv => args.Any(a => kv.Value.IsMatch(a))).Select(static kv => kv.Key).ToArray();
    }

    private static void BuildBot(string outputDirectory, Runtime runtime)
        => StartBuildProcess(outputDirectory, runtime, _botProjectPath, "OkayegTeaTime");

    private static void StartBuildProcess(string outputDir, Runtime runtime, string projectPath, string projectName)
    {
        using Process buildProcess = new();
        buildProcess.StartInfo = new("dotnet", $"publish -r {runtime.Identifier} -c Release -o {outputDir} --no-self-contained {projectPath}")
        {
            RedirectStandardOutput = false,
            RedirectStandardError = false
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

    private static void DeleteDirectory(string directory)
    {
        Console.WriteLine($"Deleting directory: \"{directory}\"{Environment.NewLine}");
        if (!Directory.Exists(directory))
        {
            return;
        }

        Directory.Delete(directory, true);
    }

    private static void CreateLastCommitFile()
    {
        Console.WriteLine("Retrieving last commit id");
        string[] lines = File.ReadAllLines(_commitIdSourcePath);
        string commitId = lines[^1].Split(' ')[1][..7];
        Console.WriteLine($"Last commit: {commitId}");
        File.WriteAllText(_commitIdFile, commitId);
        Console.WriteLine($"Created \"LastCommit\" file{Environment.NewLine}");
    }

    private static void CreateCodeFilesFile()
    {
        Console.WriteLine("Searching for .cs files");
        Regex fileRegex = new($@"^\.[\\/]{AppSettings.AssemblyName.Split('.')[0]}(\.\w+)?[\\/](?!((bin)|(obj)[\\/])).*\.cs$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        string[] files = Directory.GetFiles(".", "*", SearchOption.AllDirectories).Where(f => fileRegex.IsMatch(f)).Select(static f => f[2..].Replace('\\', '/')).Order().ToArray();
        Console.WriteLine($"Found {files.Length} .cs files");
        File.WriteAllLines(_codeFilesFile, files);
        Console.WriteLine($"Created \"CodeFiles\" file{Environment.NewLine}");
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
