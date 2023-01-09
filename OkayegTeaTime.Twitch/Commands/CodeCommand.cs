using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using HLE;
using HLE.Collections;
using OkayegTeaTime.Files;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Code)]
public readonly unsafe ref struct CodeCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private static string[]? _codeFiles;

    public CodeCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
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
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Regex? filePattern;
            try
            {
                filePattern = new(ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..], RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
            }
            catch (Exception)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.TheGivenPatternIsInvalid);
                return;
            }

            string[] matchingFiles = _codeFiles!.Where(f => filePattern.IsMatch(f)).ToArray();
            Span<char> lengthChars = stackalloc char[30];
            matchingFiles.Length.TryFormat(lengthChars, out int lengthLength);
            lengthChars = lengthChars[..lengthLength];
            switch (matchingFiles.Length)
            {
                case 0:
                    Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YourPatternMatchedNoSourceCodeFiles);
                    break;
                case 1:
                    Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, AppSettings.RepositoryUrl, "/blob/master/", matchingFiles[0]);
                    break;
                case <= 5:
                    Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, "your pattern matched ", lengthChars, " files: ", matchingFiles.JoinToString(PredefinedMessages.CommaSpace), ". Please specify");
                    break;
                default:
                    Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, "your pattern matched too many (", lengthChars, ") files. Please specify");
                    break;
            }
        }
    }
}
