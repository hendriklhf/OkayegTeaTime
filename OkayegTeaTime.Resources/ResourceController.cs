using System;
using System.Reflection;
using System.Text;
using HLE.Resources;

namespace OkayegTeaTime.Resources;

public static class ResourceController
{
    private static readonly ResourceReader _resourceReader = new(Assembly.GetExecutingAssembly());

    public static string Commands { get; } = Encoding.UTF8.GetString(_resourceReader.Read("Commands.json")!);

    public static string CSharpTemplate { get; } = Encoding.UTF8.GetString(_resourceReader.Read("CSharpTemplate.cs")!);

    public static string GachiSongs { get; } = Encoding.UTF8.GetString(_resourceReader.Read("GachiSongs.json")!);

    public static string LastCommit { get; } = Encoding.UTF8.GetString(_resourceReader.Read("LastCommit")!);

    public static string CodeFiles { get; } = Encoding.UTF8.GetString(_resourceReader.Read("CodeFiles")!);

    public static string KotlinTemplate { get; } = Encoding.UTF8.GetString(_resourceReader.Read("KotlinTemplate.kt")!);

    public static string HangmanWords { get; } = Encoding.UTF8.GetString(_resourceReader.Read("HangmanWords")!.AsSpan(..^2));
}
