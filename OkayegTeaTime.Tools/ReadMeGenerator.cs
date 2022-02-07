using System.IO;
using System.Linq;
using System.Text;
using HLE.Collections;
using OkayegTeaTime.Files;

namespace OkayegTeaTime.Tools;

public class ReadMeGenerator
{
    private readonly string[] _header1Text =
    {
        "If your channel has a custom prefix set, commands will have to start with the prefix.",
        "If your channel has no prefix set, commands will have to end with \"eg\", for example: \"pingeg\".",
        "Text in \"[ ]\" is a variable parameter"
    };

    private readonly string[] _cmdTableHeader =
    {
        "Command",
        "Alias",
        "Description [Parameter | Output]"
    };

    private readonly string[] _afkCmdTableHeader =
    {
        "Command",
        "Alias",
        "Parameter",
        "Description"
    };

    private const string _title = "OkayegTeaTime";
    private const string _header1 = "Commands";
    private const string _header2 = "Afk-Commands";

    private const string _readMePath = "./README.md";
    private const string _jsonPath = "./OkayegTeaTime/Resources/Commands.json";

    public void GenerateReadMe()
    {
        string readme = CreateReadMe();
        WriteToFile(readme);
    }

    private void WriteToFile(string readme)
    {
        File.WriteAllText(_readMePath, readme);
    }

    private string CreateReadMe()
    {
        JsonController.Initialize();
        StringBuilder builder = new();
        builder.Append($"<h1>{_title}</h1><h2>{_header1}</h2>");
        _header1Text.ForEach(h => builder.Append($"{h}<br/>"));
        builder.Append("<br/><table><tr>");
        _cmdTableHeader.ForEach(h => builder.Append($"<th>{h}</th>"));
        builder.Append("<tr/>");
        AppSettings.CommandList.Commands.OrderBy(c => c.Name).ForEach(c =>
        {
            if (c.Document)
            {
                builder.Append($"<tr><td>{c.Name}</td><td><table>");
                c.Alias.ForEach(a => builder.Append($"<tr><td>{a}</td></tr>"));
                builder.Append("</table></td><td><table>");
                for (int i = 0; i < c.Parameter.Count; i++)
                {
                    builder.Append($"<tr><td>{c.Parameter[i]}</td><td>{c.Description[i]}</td></tr>");
                }
                builder.Append("</table></td></tr>");
            }
        });
        builder.Append($"</table><h2>{_header2}</h2><table><tr>");
        _afkCmdTableHeader.ForEach(h => builder.Append($"<th>{h}</th>"));
        builder.Append("</tr>");
        AppSettings.CommandList.AfkCommands.OrderBy(c => c.Name).ForEach(c =>
        {
            builder.Append($"<tr><td>{c.Name}</td><td><table>");
            c.Alias.ForEach(a => builder.Append($"<tr><td>{a}</td></tr>"));
            builder.Append($"</table></td>");
            for (int i = 0; i < c.Parameter.Count; i++)
            {
                builder.Append($"<td>{c.Parameter[i]}</td><td>{c.Description[i]}</td>");
            }
            builder.Append("</td></tr>");
        });
        builder.Append("</table>");
        return builder.ToString();
    }
}
