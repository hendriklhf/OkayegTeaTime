using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Resources;

public static class ResourceController
{
    [FileName("Commands.json")]
    [FieldName(nameof(_commands))]
    public static string Commands
    {
        get => GetFileContent(nameof(Commands));
        private set => _commands = value;
    }

    [FileName("CompilerTemplateCSharp.cs")]
    [FieldName(nameof(_compilerTemplateCSharp))]
    public static string CompilerTemplateCSharp
    {
        get => GetFileContent(nameof(CompilerTemplateCSharp));
        private set => _compilerTemplateCSharp = value;
    }

    [FileName("CompilerTemplateGo.go")]
    [FieldName(nameof(_compilerTemplateGo))]
    public static string CompilerTemplateGo
    {
        get => GetFileContent(nameof(CompilerTemplateGo));
        private set => _compilerTemplateGo = value;
    }

    [FileName("GachiSongs.json")]
    [FieldName(nameof(_gachiSongs))]
    public static string GachiSongs
    {
        get => GetFileContent(nameof(GachiSongs));
        private set => _gachiSongs = value;
    }

    [FileName("CompilerTemplateCpp.cpp")]
    [FieldName(nameof(_compilerTemplateCpp))]
    public static string CompilerTemplateCpp
    {
        get => GetFileContent(nameof(CompilerTemplateCpp));
        private set => _compilerTemplateCpp = value;
    }

    [FileName("LastCommit")]
    [FieldName(nameof(_lastCommit))]
    public static string LastCommit
    {
        get => GetFileContent(nameof(LastCommit));
        private set => _lastCommit = value;
    }

    private static string? _commands;
    private static string? _compilerTemplateCSharp;
    private static string? _compilerTemplateGo;
    private static string? _compilerTemplateCpp;
    private static string? _gachiSongs;
    private static string? _lastCommit;

    private static readonly Assembly _assembly = Assembly.GetCallingAssembly();
    private static readonly Regex _fileEndingPattern = new(@"\.[^\.]+$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    private static string GetFileContent(string propertyName)
    {
        string? content = GetFieldValue(propertyName);
        if (content is not null)
        {
            return content;
        }

        string fileName = GetFileName(propertyName);
        string resourcePath = string.Join('.', _assembly.GetName().Name, fileName);
        using Stream stream = _assembly.GetManifestResourceStream(resourcePath)!;
        using StreamReader reader = new(stream);
        content = reader.ReadToEnd();
        PropertyInfo property = typeof(ResourceController).GetProperty(_fileEndingPattern.Replace(fileName, string.Empty))!;
        property.SetValue(null, content);
        return content;
    }

    private static string GetFileName(string propertyName)
    {
        PropertyInfo property = typeof(ResourceController).GetProperty(propertyName)!;
        FileName fileName = property.GetCustomAttribute<FileName>()!;
        return fileName.Value;
    }

    private static string? GetFieldValue(string propertyName)
    {
        PropertyInfo property = typeof(ResourceController).GetProperty(propertyName)!;
        FieldName attr = property.GetCustomAttribute<FieldName>()!;
        FieldInfo field = typeof(ResourceController).GetField(attr.Name)!;
        return (string?)field.GetValue(null);
    }
}
