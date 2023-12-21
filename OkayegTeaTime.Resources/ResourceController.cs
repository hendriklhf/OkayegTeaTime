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

    private static string? s_csharpTemplate;
    private static string? s_gachiSongs;
    private static string? s_lastCommit;
    private static string? s_codeFiles;
    private static string? s_hangmanWords;

    private const string CSharpTemplateResource = "CSharpTemplate.cs";
    private const string GachiSongsResource = "GachiSongs.json";
    private const string LastCommitResource = "LastCommit";
    private const string CodeFilesResource = "CodeFiles";
    private const string HangmanWordsResource = "HangmanWords";

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
        => throw new InvalidOperationException($"Resource {resourceName} not found.");
}
