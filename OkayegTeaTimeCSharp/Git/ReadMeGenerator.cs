using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Properties;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OkayegTeaTimeCSharp.Git
{
    public static class ReadMeGenerator
    {
        private static readonly string _htmlPath = Resources.ReadMePath;
        private static readonly string _title = "OkayegTeaTime";
        private static readonly string _header1 = "Commands";
        private static readonly string _header2 = "AFK-Commands";
        private static readonly List<string> _header1Text = new()
        {
            "If your channel has a custom prefix set, commands will have to start with the prefix.",
            "If your channel has no prefix set, commands will have to end with \"eg\", for example: \"pingeg\".",
            "Text in \"[ ]\" is a variable parameter"
        };
        private static readonly List<string> _cmdTableHeader = new()
        {
            "Command",
            "Alias",
            "Description [Parameter | Output]"
        };
        private static readonly List<string> _afkCmdTableHeader = new()
        {
            "Command",
            "Alias",
            "Parameter",
            "Description"
        };

        public static void GenerateReadMe()
        {
            File.WriteAllText(_htmlPath, GenerateString());
        }

        private static string GenerateString()
        {
            string result = string.Empty;
            result += Header(1, _title);
            result += Header(2, _header1);
            _header1Text.ForEach(str =>
            {
                result += str + LineBreak();
            });
            result += LineBreak();
            result += "<table><tr>";
            _cmdTableHeader.ForEach(str =>
            {
                result += TableHeader(str);
            });
            result += "</tr>";
            JsonHelper.BotData.CommandLists.Commands.OrderBy(cmd => cmd.CommandName).ToList().ForEach(cmd =>
            {
                result += $"<tr><td>{cmd.CommandName}</td><td><table>";
                cmd.Alias.ForEach(alias =>
                {
                    result += $"<tr><td>{alias}</td></tr>";
                });
                result += "</table></td><td><table>";
                for (int i = 0; i <= cmd.Parameter.Count - 1; i++)
                {
                    result += $"<tr><td>{cmd.Parameter[i]}</td><td>{cmd.Description[i]}</td></tr>";
                }
                result += "</table></td></tr>";
            });
            result += "</table>";
            result += Header(2, _header2);
            result += "<table><tr>";
            _afkCmdTableHeader.ForEach(str =>
            {
                result += $"<th>{str}</th>";
            });
            result += "</tr>";
            JsonHelper.BotData.CommandLists.AfkCommands.OrderBy(cmd => cmd.CommandName).ToList().ForEach(cmd =>
            {
                result += $"<tr><td>{cmd.CommandName}</td><td><table>";
                cmd.Alias.OrderBy(alias => alias).ToList().ForEach(alias =>
                {
                    result += $"<tr><td>{alias}</td></tr>";
                });
                result += "</table></td>";
                for (int i = 0; i <= cmd.Parameter.Count - 1; i++)
                {
                    result += $"<td>{cmd.Parameter[i]}</td><td>{cmd.Description[i]}</td>";
                }
                result += "</tr>";
            });
            result += "</table>";
            return result;
        }

        private static string Header(int i, string text)
        {
            return $"<h{i}>{text}</h{i}>";
        }

        private static string LineBreak()
        {
            return "<br />";
        }

        private static string TableHeader(string content)
        {
            return $"<th>{content}</th>";
        }
    }
}
