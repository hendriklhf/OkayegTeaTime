using OkayegTeaTimeCSharp.Properties;
using System.Collections.Generic;
using System.IO;

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
        private static readonly List<string> _tableHeader = new()
        {
            "Command",
            "Alias",
            "Description [Parameter | Output]"
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
            result += "<table>";
            //content
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

        private static string TableRow(string content)
        {
            return $"<tr>{content}</tr>";
        }

        private static string TableHeader(string content)
        {
            return $"<th>{content}</th>";
        }
    }
}
