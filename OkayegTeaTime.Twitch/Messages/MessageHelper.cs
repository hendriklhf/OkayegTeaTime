using System;
using System.Runtime.CompilerServices;
using OkayegTeaTime.Settings;

namespace OkayegTeaTime.Twitch.Messages;

public static class MessageHelper
{
    public static void ExtractAlias(ReadOnlyMemory<char> message, ReadOnlySpan<char> channelPrefix, out ReadOnlyMemory<char> usedAlias, out ReadOnlyMemory<char> usedPrefix)
    {
        ReadOnlySpan<char> messageSpan = message.Span;
        int indexOfWhitespace = messageSpan.IndexOf(' ');
        ReadOnlyMemory<char> firstWord = message[..Unsafe.As<int, Index>(ref indexOfWhitespace)];
        if (firstWord.Length <= (channelPrefix.Length == 0 ? AppSettings.Suffix.Length : channelPrefix.Length))
        {
            usedAlias = ReadOnlyMemory<char>.Empty;
            usedPrefix = ReadOnlyMemory<char>.Empty;
            return;
        }

        if (channelPrefix.Length == 0)
        {
            usedAlias = firstWord[..^AppSettings.Suffix.Length];
            usedPrefix = firstWord[^AppSettings.Suffix.Length..];
            return;
        }

        usedAlias = firstWord[channelPrefix.Length..];
        usedPrefix = firstWord[..channelPrefix.Length];
    }
}
