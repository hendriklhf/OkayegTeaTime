using System.Reflection;
using HLE.Resources;

namespace OkayegTeaTime.Resources;

public static class ResourceController
{
    public static string Commands => _resourceReader.ReadResource("Commands.json")!;

    public static string CSharpTemplate => _resourceReader.ReadResource("CSharpTemplate.cs")!;

    public static string GachiSongs => _resourceReader.ReadResource("GachiSongs.json")!;

    public static string LastCommit => _resourceReader.ReadResource("LastCommit")!;

    public static string CodeFiles => _resourceReader.ReadResource("CodeFiles")!;

    public static string KotlinTemplate => _resourceReader.ReadResource("KotlinTemplate.kt")!;

    public static string HangmanWords => _resourceReader.ReadResource("HangmanWords")!;

    private static readonly ResourceReader _resourceReader = new(Assembly.GetExecutingAssembly());
}
