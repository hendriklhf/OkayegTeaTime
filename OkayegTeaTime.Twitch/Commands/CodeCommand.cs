using System;
using System.Linq;
using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Files;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Code)]
public readonly unsafe ref struct CodeCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

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
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Regex? filePattern;
            try
            {
                filePattern = new(ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..], RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
            }
            catch (ArgumentException)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.TheGivenPatternIsInvalid);
                return;
            }

            string[] matchingFiles = _codeFiles!.Where(f => filePattern.IsMatch(f)).ToArray();
            Response->Append(ChatMessage.Username, Messages.CommaSpace);
            switch (matchingFiles.Length)
            {
                case 0:
                    Response->Append(Messages.YourPatternMatchedNoSourceCodeFiles);
                    break;
                case 1:
                    Response->Append(AppSettings.RepositoryUrl, "/blob/master/", matchingFiles[0]);
                    break;
                case <= 5:
                    Span<char> joinBuffer = stackalloc char[500];
                    int bufferLength = StringHelper.Join(matchingFiles, Messages.CommaSpace, joinBuffer);
                    Response->Append("your pattern matched ");
                    Response->Append(matchingFiles.Length);
                    Response->Append(" files: ", joinBuffer[..bufferLength], ". Please specify");
                    break;
                default:
                    Response->Append("your pattern matched too many (");
                    Response->Append(matchingFiles.Length);
                    Response->Append(") files. Please specify");
                    break;
            }
        }
    }
}
