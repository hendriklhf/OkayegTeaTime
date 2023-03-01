using System;
using System.Linq;
using System.Text.RegularExpressions;
using HLE;
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
    private readonly string? _prefix;
    private readonly string _alias;

    private static string[]? _codeFiles;

    public CodeCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;

        _codeFiles ??= ResourceController.CodeFiles.Split(new[]
        {
            "\r\n",
            "\n",
            "\r"
        }, StringSplitOptions.RemoveEmptyEntries);
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

            string[] matchingFiles = _codeFiles!.Where(f => filePattern.IsMatch(f)).ToArray();
            _response.Append(ChatMessage.Username, ", ");
            switch (matchingFiles.Length)
            {
                case 0:
                    _response.Append(Messages.YourPatternMatchedNoSourceCodeFiles);
                    break;
                case 1:
                    _response.Append(AppSettings.RepositoryUrl, "/blob/master/", matchingFiles[0]);
                    break;
                case <= 5:
                    Span<char> joinBuffer = stackalloc char[500];
                    int bufferLength = StringHelper.Join(matchingFiles, ", ", joinBuffer);
                    _response.Append("your pattern matched ");
                    _response.Append(matchingFiles.Length);
                    _response.Append(" files: ", joinBuffer[..bufferLength], ". Please specify");
                    break;
                default:
                    _response.Append("your pattern matched too many (");
                    _response.Append(matchingFiles.Length);
                    _response.Append(") files. Please specify");
                    break;
            }
        }
    }
}
