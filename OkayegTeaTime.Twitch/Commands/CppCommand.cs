using System.Text.RegularExpressions;
using System.Web;
using HLE;
using HLE.Http;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

#if DEBUG
[HandledCommand(CommandType.Cpp)]
#endif
public sealed class CppCommand : Command
{
    public CppCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string code = ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..];
            string requestResult = GetCppOnlineCompilerResult(code);
            Response = $"{ChatMessage.Username}, {requestResult}";
        }
    }

    private static string GetCppOnlineCompilerResult(string code)
    {
        HttpPost request = new("https://tpcg2.tutorialspoint.com/tpcg.php", new[]
        {
            ("lang", "cpp"),
            ("device", string.Empty),
            ("code", GetCppOnlineCompilerTemplate(code)),
            ("stdinput", string.Empty),
            ("ext", "cpp"),
            ("compile", HttpUtility.HtmlEncode("g++ -o main *.cpp")),
            ("execute", "main"),
            ("mainfile", "main.cpp"),
            ("uid", "968291")
        });

        if (request.Result is null)
        {
            return "compiler service error";
        }

        string result = Regex.Match(request.Result.NewLinesToSpaces(), @"\$main(</b>|</span>|<br>){3}.*").Value[20..].NewLinesToSpaces().TrimAll();
        return string.IsNullOrWhiteSpace(result) ? "compiled successfully" : result.Length > 450 ? $"{result[450..]}..." : result;
    }

    private static string GetCppOnlineCompilerTemplate(string code)
    {
        return ResourceController.CompilerTemplateCpp.Replace("{code}", code);
    }
}
