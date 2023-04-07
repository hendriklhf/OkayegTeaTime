using System;
using System.Text.RegularExpressions;
using HLE;
using HLE.Collections;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Code)]
public readonly ref struct CodeCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref MessageBuilder _response;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    private static string[]? _codeFiles;

    public CodeCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;

        _codeFiles ??= ResourceController.CodeFiles.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Regex? filePattern;
            try
            {
                using ChatMessageExtension messageExtension = new(ChatMessage);
                filePattern = new(ChatMessage.Message[(messageExtension.Split[0].Length + 1)..], RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
            }
            catch (ArgumentException)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.TheGivenPatternIsInvalid);
                return;
            }

            using PoolBufferList<string> matchingFiles = new(5);
            GetMatchingFiles(filePattern, matchingFiles);
            _response.Append(ChatMessage.Username, ", ");
            switch (matchingFiles.Count)
            {
                case 0:
                    _response.Append(Messages.YourPatternMatchedNoSourceCodeFiles);
                    break;
                case 1:
                    _response.Append(AppSettings.RepositoryUrl, "/blob/master/", matchingFiles[0]);
                    break;
                case <= 5:
                    _response.Append("your pattern matched ");
                    _response.Append(matchingFiles.Count);
                    _response.Append(" files: ");

                    int joinLength = StringHelper.Join(matchingFiles.AsSpan(), ", ", _response.FreeBuffer);
                    _response.Advance(joinLength);

                    _response.Append(". Please specify.");
                    break;
                default:
                    _response.Append("your pattern matched too many (");
                    _response.Append(matchingFiles.Count);
                    _response.Append(") files. Please specify.");
                    break;
            }
        }
    }

    private static void GetMatchingFiles(Regex filePattern, PoolBufferList<string> files)
    {
        for (int i = 0; i < _codeFiles!.Length; i++)
        {
            string file = _codeFiles[i];
            if (filePattern.IsMatch(file))
            {
                files.Add(file);
            }
        }
    }
}
