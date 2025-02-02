using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using HLE.Resources;

namespace OkayegTeaTime.Resources;

public static class ResourceController
{
    private static readonly ResourceReader s_resourceReader = new(typeof(ResourceController).Assembly);

    public static string CSharpTemplate => s_csharpTemplate ??= ReadString(CSharpTemplateResource);

    public static string GachiSongs => s_gachiSongs ??= ReadString(GachiSongsResource);

    public static string LastCommit => s_lastCommit ??= ReadString(LastCommitResource);

    public static string CodeFiles => s_codeFiles ??= ReadString(CodeFilesResource);

    public static string HangmanWords => s_hangmanWords ??= ReadHangmanWords();

#pragma warning disable IDE0032
    private static string? s_csharpTemplate;
    private static string? s_gachiSongs;
    private static string? s_lastCommit;
    private static string? s_codeFiles;
    private static string? s_hangmanWords;
#pragma warning restore IDE0032

    private const string ResourceNamespace = "OkayegTeaTime.Resources.";
    private const string CSharpTemplateResource = $"{ResourceNamespace}CSharpTemplate.cs";
    private const string GachiSongsResource = $"{ResourceNamespace}GachiSongs.json";
    private const string LastCommitResource = $"{ResourceNamespace}LastCommit";
    private const string CodeFilesResource = $"{ResourceNamespace}CodeFiles";
    private const string HangmanWordsResource = $"{ResourceNamespace}HangmanWords";

    private static string ReadHangmanWords()
    {
        bool success = s_resourceReader.TryRead(HangmanWordsResource, out Resource resource);
        if (!success)
        {
            ThrowResourceNotFound(HangmanWordsResource);
        }

        return Encoding.UTF8.GetString(resource.AsSpan()[..^2]);
    }

    private static string ReadString(string resourceName)
    {
        bool success = s_resourceReader.TryRead(resourceName, out Resource resource);
        if (!success)
        {
            ThrowResourceNotFound(resourceName);
        }

        return Encoding.UTF8.GetString(resource.AsSpan());
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowResourceNotFound(string resourceName)
        => throw new InvalidOperationException($"Resource \"{resourceName}\" not found.");
}
