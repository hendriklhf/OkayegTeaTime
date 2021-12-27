using System.IO;
using System.Reflection;

namespace OkayegTeaTime.Files;

public static class FileController
{
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
    private const string _resourceFolder = "Resources";
    private static readonly string[] _fileNames =
    {
        NewFile("Commands.json"),
        NewFile("ConnectionString.json"),
        NewFile("OnlineCompilerTemplate.cs"),
        NewFile("OnlineCompilerTemplate.go"),
        NewFile("RandomWords.json")
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

    private static string NewFile(string fileName)
    {
        return string.Join('.', AppSettings.AssemblyName, _resourceFolder, fileName);
    }
}
