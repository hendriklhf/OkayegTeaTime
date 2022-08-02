using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HLE.Collections;

namespace OkayegTeaTime.Files;

public static class FileLocator
{
    private static string[]? _files;

    /// <summary>
    /// Searches for a file in the current and all sub directories and returns the path of it.
    /// </summary>
    /// <exception cref="FileNotFoundException">
    /// Thrown if a file couldn't be found.
    /// </exception>
    public static string Find(string fileName)
    {
        _files ??= BuildFileCache();
        Regex filePattern = new($@"[\/\\]{fileName}$", RegexOptions.Compiled);
        return _files.FirstOrDefault(f => filePattern.IsMatch(f)) ?? throw new FileNotFoundException($"The file {fileName} could not be found in the current or any sub directory.");
    }

    private static string[] BuildFileCache()
    {
        List<string> files = new();
        GetFiles(Directory.GetCurrentDirectory(), files);
        return files.OrderBy(f => f.Count(c => c is '\\' or '/')).ToArray();
    }

    private static void GetFiles(string directory, List<string> fileList)
    {
        string[] dirs = Directory.GetDirectories(directory);
        dirs.ForEach(d => GetFiles(d, fileList));

        string[] files = Directory.GetFiles(directory);
        fileList.AddRange(files);
    }
}
