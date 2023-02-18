using System.Reflection;
using HLE.Resources;

namespace OkayegTeaTime.Resources;

public static class ResourceController
{
    public static string Commands => _resourceReader.Read("Commands.json")!;

    public static string CSharpTemplate => _resourceReader.Read("CSharpTemplate.cs")!;

    public static string GachiSongs => _resourceReader.Read("GachiSongs.json")!;

    public static string LastCommit => _resourceReader.Read("LastCommit")!;

    public static string CodeFiles => _resourceReader.Read("CodeFiles")!;

    public static string KotlinTemplate => _resourceReader.Read("KotlinTemplate.kt")!;

    public static string HangmanWords => _resourceReader.Read("HangmanWords")!;

    private static readonly ResourceReader _resourceReader = new(Assembly.GetExecutingAssembly());
}
