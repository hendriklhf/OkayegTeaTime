using System.IO;
using System.Reflection;

namespace OkayegTeaTime.Resources;

public static class ResourceController
{
    [FileName("Commands.json")]
    public static string Commands
    {
        get => GetFileContent(nameof(Commands), nameof(_commands));
        private set => _commands = value;
    }

    [FileName("CompilerTemplateCSharp.cs")]
    public static string CompilerTemplateCSharp
    {
        get => GetFileContent(nameof(CompilerTemplateCSharp), nameof(_compilerTemplateCSharp));
        private set => _compilerTemplateCSharp = value;
    }

    [FileName("CompilerTemplateGo.go")]
    public static string CompilerTemplateGo
    {
        get => GetFileContent(nameof(CompilerTemplateGo), nameof(_compilerTemplateGo));
        private set => _compilerTemplateGo = value;
    }

    [FileName("GachiSongs.json")]
    public static string GachiSongs
    {
        get => GetFileContent(nameof(GachiSongs), nameof(_gachiSongs));
        private set => _gachiSongs = value;
    }

    [FileName("CompilerTemplateCpp.cpp")]
    public static string CompilerTemplateCpp
    {
        get => GetFileContent(nameof(CompilerTemplateCpp), nameof(_compilerTemplateCpp));
        private set => _compilerTemplateCpp = value;
    }

    [FileName("LastCommit")]
    public static string LastCommit
    {
        get => GetFileContent(nameof(LastCommit), nameof(_lastCommit));
        private set => _lastCommit = value;
    }

    private static string? _commands;
    private static string? _compilerTemplateCSharp;
    private static string? _compilerTemplateGo;
    private static string? _compilerTemplateCpp;
    private static string? _gachiSongs;
    private static string? _lastCommit;

    private static readonly Assembly _assembly = Assembly.GetCallingAssembly();

    private static string GetFileContent(string propertyName, string fieldName)
    {
        string? content = GetFieldValue(fieldName);
        if (content is not null)
        {
            return content;
        }

        string fileName = GetFileName(propertyName);
        string resourcePath = string.Join('.', _assembly.GetName().Name, fileName);
        using Stream stream = _assembly.GetManifestResourceStream(resourcePath)!;
        using StreamReader reader = new(stream);
        content = reader.ReadToEnd();
        PropertyInfo property = typeof(ResourceController).GetProperty(propertyName)!;
        property.SetValue(null, content);
        return content;
    }

    private static string GetFileName(string propertyName)
    {
        PropertyInfo property = typeof(ResourceController).GetProperty(propertyName)!;
        FileName fileName = property.GetCustomAttribute<FileName>()!;
        return fileName.Value;
    }

    private static string? GetFieldValue(string fieldName)
    {
        FieldInfo field = typeof(ResourceController).GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic)!;
        return (string?)field.GetValue(null);
    }
}
