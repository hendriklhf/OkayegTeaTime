using System;
using System.IO;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Tools;

public sealed class ResourceSyncer
{
    private readonly string _cloudPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/iCloudDrive";
    private const string _relativeCloudResourcePath = "/Development/OkayegTeaTime/Resources";
    private readonly string _resourcePath = $"{Directory.GetCurrentDirectory()}/OkayegTeaTime.Resources";

    private readonly Regex _fileNamePattern = NewRegex(@"[\\/][^\\/]+$");

    public void Download()
    {
        if (!CheckForPathExistance())
        {
            return;
        }

        string[] cloudFiles = Directory.GetFiles(_cloudPath + _relativeCloudResourcePath);
        foreach (string file in cloudFiles)
        {
            File.Copy(file, $"{_resourcePath}/{_fileNamePattern.Match(file).Value[1..]}", true);
        }

        Console.WriteLine($"{cloudFiles.Length} files have been downloaded.");
    }

    public void Upload()
    {
        if (!CheckForPathExistance())
        {
            return;
        }

        string[] files = Directory.GetFiles(_resourcePath);
        foreach (string file in files)
        {
            File.Copy(file, $"{_cloudPath + _relativeCloudResourcePath}/{_fileNamePattern.Match(file).Value[1..]}", true);
        }

        Console.WriteLine($"{files.Length} files have been uploaded.");
    }

    private bool CheckForPathExistance()
    {
        if (!Directory.Exists(_cloudPath))
        {
            Console.WriteLine($"iCloud is not installed on this device. \"{_cloudPath}\" could not be found.");
            return false;
        }

        string cloudResourcePath = _cloudPath + _relativeCloudResourcePath;
        if (!Directory.Exists(cloudResourcePath))
        {
            Directory.CreateDirectory(cloudResourcePath);
        }

        if (!Directory.Exists(_resourcePath))
        {
            Console.WriteLine($"Directory \"{_resourcePath}\" could not be found.");
            return false;
        }

        return true;
    }
}
