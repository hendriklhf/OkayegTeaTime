using System.IO;
using System.Reflection;

namespace OkayegTeaTime.Files;

public static class FileController
{
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
    private const string _resourceFolder = "Resources";
    private static readonly string[] _fileNames =
    {
        $"{AppSettings.AssemblyName}.{_resourceFolder}.Commands.json",
        $"{AppSettings.AssemblyName}.{_resourceFolder}.ConnectionString.json",
        $"{AppSettings.AssemblyName}.{_resourceFolder}.OnlineCompilerTemplate.cs",
        $"{AppSettings.AssemblyName}.{_resourceFolder}.OnlineCompilerTemplate.go",
        $"{AppSettings.AssemblyName}.{_resourceFolder}.RandomWords.json",
    };

    public static string Commands { get; private set; } = Read(_fileNames[0]);

    public static string ConnectionString { get; private set; } = Read(_fileNames[1]);

    public static string OnlineCompilerTemplateCSharp { get; private set; } = Read(_fileNames[2]);

    public static string OnlineCompilerTemplateGo { get; private set; } = Read(_fileNames[3]);

    public static string RandomWords { get; private set; } = Read(_fileNames[4]);

    private static string Read(string resourcePath)
    {
        using Stream stream = _assembly.GetManifestResourceStream(resourcePath);
        using StreamReader reader = new(stream);
        string content = reader.ReadToEnd();
        return content;
    }
}
