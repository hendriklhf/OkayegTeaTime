﻿using System.IO;
using System.Reflection;

#nullable disable

namespace OkayegTeaTime.Files;

public static class FileController
{
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
    private const string _resourceFolder = "Resources";
    private static readonly string[] _fileNames =
    {
        NewFile("Commands.json"),
        NewFile("OnlineCompilerTemplate.cs"),
        NewFile("OnlineCompilerTemplate.go"),
        NewFile("GachiSongs.json"),
        NewFile("OnlineCompilerTemplate.cpp")
    };

    public static string Commands { get; private set; } = Read(_fileNames[0]);

    public static string OnlineCompilerTemplateCSharp { get; private set; } = Read(_fileNames[1]);

    public static string OnlineCompilerTemplateGo { get; private set; } = Read(_fileNames[2]);

    public static string GachiSongs { get; private set; } = Read(_fileNames[3]);

    public static string OnlineCompilerTemplateCpp { get; private set; } = Read(_fileNames[4]);

    private static string Read(string resourcePath)
    {
        using Stream stream = _assembly.GetManifestResourceStream(resourcePath);
        using StreamReader reader = new(stream);
        string content = reader.ReadToEnd();
        return content;
    }

    private static string NewFile(string fileName)
    {
        return string.Join('.', AppSettings.AssemblyName, _resourceFolder, fileName);
    }
}
