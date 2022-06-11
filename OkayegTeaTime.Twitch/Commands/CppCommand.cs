#if DEBUG
using System.Text.RegularExpressions;
using System.Web;
using HLE;
using HLE.Http;
using OkayegTeaTime.Resources;
#endif
using OkayegTeaTime.Twitch.Models;
#if DEBUG
using OkayegTeaTime.Utils;
#endif

namespace OkayegTeaTime.Twitch.Commands;

public class CppCommand : Command
{
    public CppCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
#if DEBUG
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string code = ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..];
            string requestResult = GetCppOnlineCompilerResult(code);
            Response = $"{ChatMessage.Username}, {requestResult}";
        }
#endif
    }

#if DEBUG
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

        string result = Regex.Match(request.Result.NewLinesToSpaces(), @"\$main(</b>|</span>|<br>){3}.*").Value[20..].TrimAll();
        if (result.IsNullOrEmptyOrWhitespace())
        {
            return "compiled successfully";
        }

        return (result.Length > 450 ? $"{result[450..]}..." : result).NewLinesToSpaces();
    }

    private static string GetCppOnlineCompilerTemplate(string code)
    {
        return ResourceController.CompilerTemplateCpp.Replace("{code}", code);
    }
#endif
}
