using System;
using System.IO;
using System.Linq;
using System.Text;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Twitch.Controller;

namespace OkayegTeaTime.Tools;

public sealed class ReadMeGenerator
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

    public void Generate()
    {
        string readme = CreateReadMe();
        File.WriteAllText(_readMePath, readme);
        Console.WriteLine($"Created README to file: {_readMePath}");
    }

    private string CreateReadMe()
    {
        StringBuilder builder = new();
        CommandController commandController = new();

        builder.Append($"<h1>{_title}</h1><h2>{_header1}</h2>");
        foreach (string headerText in _header1Text)
        {
            builder.Append($"{headerText}<br/>");
        }

        builder.Append("<br/><table><tr>");
        foreach (string cmdHeader in _cmdTableHeader)
        {
            builder.Append($"<th>{cmdHeader}</th>");
        }

        builder.Append("<tr/>");
        Command[] commands = commandController.Commands.Where(c => c.Document).OrderBy(c => c.Name).ToArray();
        foreach (Command cmd in commands)
        {
            builder.Append($"<tr><td>{cmd.Name}</td><td><table>");
            foreach (string alias in cmd.Aliases)
            {
                builder.Append($"<tr><td>{alias}</td></tr>");
            }

            builder.Append("</table></td><td><table>");
            for (int i = 0; i < cmd.Parameters.Length; i++)
            {
                builder.Append($"<tr><td>{cmd.Parameters[i]}</td><td>{cmd.ParameterDescriptions[i]}</td></tr>");
            }

            builder.Append("</table></td></tr>");
        }

        builder.Append($"</table><h2>{_header2}</h2><table><tr>");
        foreach (string cmdHeader in _afkCmdTableHeader)
        {
            builder.Append($"<th>{cmdHeader}</th>");
        }

        builder.Append("</tr>");
        AfkCommand[] afkCommands = commandController.AfkCommands.Where(c => c.Document).OrderBy(c => c.Name).ToArray();
        foreach (AfkCommand cmd in afkCommands)
        {
            builder.Append($"<tr><td>{cmd.Name}</td><td><table>");
            foreach (string alias in cmd.Aliases)
            {
                builder.Append($"<tr><td>{alias}</td></tr>");
            }

            builder.Append("</table></td>");
            for (int i = 0; i < cmd.Parameters.Length; i++)
            {
                builder.Append($"<td>{cmd.Parameters[i]}</td><td>{cmd.ParameterDescriptions[i]}</td>");
            }

            builder.Append("</td></tr>");
        }

        builder.Append("</table>");
        return builder.ToString();
    }
}
