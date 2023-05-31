using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HLE.Emojis;
using HLE.Memory;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.Strings.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.CSharp, typeof(CSharpCommand))]
public readonly struct CSharpCommand : IChatCommand<CSharpCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public CSharpCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out CSharpCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            ReadOnlyMemory<char> message = ChatMessage.Message.AsMemory();
            message = message[(messageExtension.Split[0].Length + 1)..];

            using RentedArray<char> codeBuffer = new(message.Length);
            codeBuffer.Span.Fill(' ');
            message.CopyTo(codeBuffer);
            Memory<char> code = ReplaceSpecialChars(codeBuffer);

            ReadOnlyMemory<char> result = await GetProgramOutput(new(code.Span));
            Response.Append(ChatMessage.Username, ", ", result.Span);
        }
    }

    private static async ValueTask<ReadOnlyMemory<char>> GetProgramOutput(string code)
    {
        try
        {
            string codeBlock = HttpUtility.HtmlEncode(ResourceController.CSharpTemplate.Replace("{code}", code));
            HttpPost request = await HttpPost.GetStringAsync("https://dotnetfiddle.net/home/run", new[]
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
            ReadOnlyMemory<char> output = json.GetProperty("ConsoleOutput").GetString()!.NewLinesToSpaces().AsMemory();
            switch (output.Length)
            {
                case 0:
                    return "executed successfully".AsMemory();
                case < 453:
                    return output;
            }

            Memory<char> mutableOutput = output.AsMutableMemory()[..450];
            "...".CopyTo(mutableOutput[^3..].Span);
            return output[..450];
        }
        catch (Exception ex)
        {
            if (code.Contains('\''))
            {
                return ("for some reason the API doesn't like the char \"'\". Please try to avoid it. " + Emoji.SlightlySmilingFace).AsMemory();
            }

            await DbController.LogExceptionAsync(ex);
            return "an unexpected error occurred. The API might not be available.".AsMemory();
        }
    }

    private static Memory<char> ReplaceSpecialChars(Memory<char> code)
    {
        ReadOnlySpan<char> charsToReplace = StringHelper.AntipingChar;
        Span<char> codeSpan = code.Span;
        for (int i = 0; i < charsToReplace.Length; i++)
        {
            codeSpan.Replace(charsToReplace[i], ' ');
        }

        return code.TrimEnd();
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
