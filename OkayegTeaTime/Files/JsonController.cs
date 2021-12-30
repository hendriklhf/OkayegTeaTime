using System.IO;
using System.Text.Json;
using OkayegTeaTime.Files.JsonClasses.CommandData;
using OkayegTeaTime.Files.JsonClasses.Settings;

namespace OkayegTeaTime.Files;

public static class JsonController
{
    public static Settings Settings { get; private set; }

    public static CommandList CommandList { get; private set; }

    public static List<string> RandomWords { get; private set; } = JsonSerializer.Deserialize<List<string>>(FileController.RandomWords);

    public static void Initialize()
    {
        Settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(Path.Settings));
        CommandList = JsonSerializer.Deserialize<CommandList>(FileController.Commands);
    }
}
