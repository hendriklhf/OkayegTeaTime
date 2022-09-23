using System.IO;
using System.Reflection;

namespace OkayegTeaTime.Resources;

public static class ResourceController
{
    [FileName("Commands.json")]
    public static string Commands
    {
        get => GetFileContent(nameof(Commands), ref _commands);
        private set => _commands = value;
    }

    [FileName("CompilerTemplateCSharp.cs")]
    public static string CompilerTemplateCSharp
    {
        get => GetFileContent(nameof(CompilerTemplateCSharp), ref _compilerTemplateCSharp);
        private set => _compilerTemplateCSharp = value;
    }

    [FileName("CompilerTemplateGo.go")]
    public static string CompilerTemplateGo
    {
        get => GetFileContent(nameof(CompilerTemplateGo), ref _compilerTemplateGo);
        private set => _compilerTemplateGo = value;
    }

    [FileName("GachiSongs.json")]
    public static string GachiSongs
    {
        get => GetFileContent(nameof(GachiSongs), ref _gachiSongs);
        private set => _gachiSongs = value;
    }

    [FileName("CompilerTemplateCpp.cpp")]
    public static string CompilerTemplateCpp
    {
        get => GetFileContent(nameof(CompilerTemplateCpp), ref _compilerTemplateCpp);
        private set => _compilerTemplateCpp = value;
    }

    [FileName("LastCommit")]
    public static string LastCommit
    {
        get => GetFileContent(nameof(LastCommit), ref _lastCommit);
        private set => _lastCommit = value;
    }

    [FileName("CodeFiles")]
    public static string CodeFiles
    {
        get => GetFileContent(nameof(CodeFiles), ref _codeFiles);
        private set => _codeFiles = value;
    }

    private static string? _commands;
    private static string? _compilerTemplateCSharp;
    private static string? _compilerTemplateGo;
    private static string? _compilerTemplateCpp;
    private static string? _gachiSongs;
    private static string? _lastCommit;
    private static string? _codeFiles;

    private static readonly Assembly _assembly = Assembly.GetCallingAssembly();

    private static string GetFileContent(string propertyName, ref string? field)
    {
        if (field is not null)
        {
            return field;
        }

        string fileName = GetFileName(propertyName);
        field = ReadResource(fileName);
        return field;
    }

    private static string GetFileName(string propertyName)
    {
        PropertyInfo property = typeof(ResourceController).GetProperty(propertyName)!;
        FileName fileName = property.GetCustomAttribute<FileName>()!;
        return fileName.Value;
    }

    private static string ReadResource(string fileName)
    {
        string resourcePath = string.Join('.', _assembly.GetName().Name, fileName);
        using Stream stream = _assembly.GetManifestResourceStream(resourcePath)!;
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}
