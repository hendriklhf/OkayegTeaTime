using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HLE.Collections;

namespace OkayegTeaTime.Tools;

public class Publisher
{
    private readonly string[] _args;

    private readonly Dictionary<string, Regex> _regexDic = new(new KeyValuePair<string, Regex>[]
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
        List<string> runtimes = GetRuntimes();
        if (!runtimes.Any())
        {
            Console.WriteLine("The provided runtimes aren't matching any available runtime identifier.");
            Environment.Exit(1);
            return;
        }

        foreach (string r in runtimes)
        {
            Process cmd = new();
            string outputDir = $"./Build/{r}/";
            ClearOutputDir(outputDir);
            cmd.StartInfo = new("dotnet", $"publish -r {r} -c Release -o {outputDir} --no-self-contained ./OkayegTeaTime/OkayegTeaTime.csproj");
            cmd.OutputDataReceived += (_, e) => Console.WriteLine(e.Data);
            cmd.ErrorDataReceived += (_, e) => Console.WriteLine(e.Data);
            cmd.Start();
            cmd.WaitForExit();
        }
    }

    private List<string> GetRuntimes()
    {
        List<string> result = new();
        _regexDic.ForEach(v =>
        {
            if (_args.Any(a => v.Value.IsMatch(a)))
            {
                result.Add(v.Key);
            }
        });
        return result;
    }

    private void ClearOutputDir(string dir)
    {
        if (!Directory.Exists(dir))
        {
            return;
        }

        Directory.Delete(dir, true);
    }
}
