using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using HLE.Emojis;
using HLE.Memory;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.Strings.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.CSharp)]
public readonly ref struct CSharpCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref PoolBufferStringBuilder _response;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public CSharpCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref PoolBufferStringBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            ReadOnlySpan<char> message = ChatMessage.Message;
            message = message[(messageExtension.Split[0].Length + 1)..];
            Span<char> code = stackalloc char[message.Length];
            message.CopyTo(code);
            ReplaceSpecialChars(ref code);
            ReadOnlySpan<char> result = GetProgramOutput(new(code));
            _response.Append(ChatMessage.Username, ", ", result);
        }
    }

    private static ReadOnlySpan<char> GetProgramOutput(string code)
    {
        try
        {
            string codeBlock = HttpUtility.HtmlEncode(ResourceController.CSharpTemplate.Replace("{code}", code));
            HttpPost request = new("https://dotnetfiddle.net/home/run", new[]
            {
                ("CodeBlock", codeBlock),
                ("Compiler", "Net7"),
                ("Language", "CSharp"),
                ("MvcViewEngine", "Razor"),
                ("NuGetPackageVersionIds", AppSettings.HleNugetVersionId),
                ("OriginalCodeBlock", codeBlock),
                ("OriginalFiddleId", "CsConsCore"),
                ("ProjectType", "Console"),
                ("TimeOffset", "1"),
                ("UseResultCache", "false")
            });
            if (request.Result is null)
            {
                throw new NullReferenceException("API result is null");
            }

            JsonElement json = JsonSerializer.Deserialize<JsonElement>(request.Result);
            ReadOnlySpan<char> output = json.GetProperty("ConsoleOutput").GetString()!.NewLinesToSpaces();
            switch (output.Length)
            {
                case 0:
                    return "executed successfully";
                case < 453:
                    return output;
            }

            Span<char> mutableOutput = output.AsMutableSpan()[..450];
            "...".CopyTo(mutableOutput[^3..]);
            return output[..450];
        }
        catch (Exception ex)
        {
            if (code.Contains('\''))
            {
                return "for some reason the API doesn't like the char \"'\". Please try to avoid it. " + Emoji.SlightlySmilingFace;
            }

            DbController.LogException(ex);
            return "an unexpected error occurred. The API might not be available.";
        }
    }

    private static void ReplaceSpecialChars(ref Span<char> code)
    {
        ReadOnlySpan<char> charsToReplace = StringHelper.AntipingChar;
        for (int i = 0; i < charsToReplace.Length; i++)
        {
            code.Replace(charsToReplace[i], ' ');
        }

        code = code.TrimEnd();
    }
}
