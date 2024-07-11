using System;
using System.IO;
using System.Linq;
using System.Text;
using OkayegTeaTime.Twitch.Commands;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Tools;

internal static class ReadMeGenerator
{
    private static readonly string[] s_header1Text =
    [
        "If your channel has a custom prefix set, commands will have to start with the prefix.",
        "If your channel has no prefix set, commands will have to end with \"eg\", for example: \"pingeg\".",
        "Text in \"[ ]\" is a variable parameter"
    ];

    private static readonly string[] s_commandTableHeaders =
    [
        "Command",
        "Alias",
        "Description [Parameter | Output]"
    ];

    private static readonly string[] s_afkCommandTableHeaders =
    [
        "Command",
        "Alias",
        "Parameter",
        "Description"
    ];

    private const string Title = "OkayegTeaTime";
    private const string Header1 = "Commands";
    private const string Header2 = "Afk Commands";

    private const string ReadMePath = "./README.md";
    private const string OpenTableRowOpenTableData = "<tr><td>";
    private const string CloseTableDataOpenTableRow = "</td></tr>";

    public static void Generate()
    {
        string readme = CreateReadMe();
        File.WriteAllText(ReadMePath, readme);
        Console.WriteLine($"Created README to file: {ReadMePath}");
    }

    private static string CreateReadMe()
    {
        StringBuilder builder = new(16384);

        builder.Append($"<h1>{Title}</h1><h2>{Header1}</h2>");
        foreach (string headerText in s_header1Text)
        {
            builder.Append(headerText).Append("<br/>");
        }

        builder.Append("<br/><table><tr>");
        foreach (string commandHeader in s_commandTableHeaders)
        {
            builder.Append("<th>").Append(commandHeader).Append("</th>");
        }

        builder.Append("<tr/>");
        Command[] commands = CommandController.Commands.Where(static c => c.Document).OrderBy(static c => c.Name).ToArray();
        foreach (Command command in commands)
        {
            builder.Append(OpenTableRowOpenTableData).Append(command.Name).Append("</td><td><table>");
            foreach (string alias in command.Aliases)
            {
                builder.Append(OpenTableRowOpenTableData).Append(alias).Append(CloseTableDataOpenTableRow);
            }

            builder.Append("</table></td><td><table>");
            foreach (Parameter parameter in command.Parameters)
            {
                builder.Append(OpenTableRowOpenTableData).Append(parameter.Name).Append("</td><td>").Append(parameter.Description).Append(CloseTableDataOpenTableRow);
            }

            builder.Append("</table>").Append(CloseTableDataOpenTableRow);
        }

        builder.Append($"</table><h2>{Header2}</h2><table><tr>");
        foreach (string commandHeader in s_afkCommandTableHeaders)
        {
            builder.Append("<th>").Append(commandHeader).Append("</th>");
        }

        builder.Append("</tr>");
        AfkCommand[] afkCommands = CommandController.AfkCommands.Where(static c => c.Document).OrderBy(static c => c.Name).ToArray();
        foreach (AfkCommand afkCommand in afkCommands)
        {
            builder.Append(OpenTableRowOpenTableData).Append(afkCommand.Name).Append("</td><td><table>");
            foreach (string alias in afkCommand.Aliases)
            {
                builder.Append(OpenTableRowOpenTableData).Append(alias).Append(CloseTableDataOpenTableRow);
            }

            builder.Append("</table></td>");
            for (int i = 0; i < afkCommand.Parameters.Length; i++)
            {
                Parameter parameter = afkCommand.Parameters[i];
                builder.Append("<td>").Append(parameter.Name).Append("</td><td>").Append(parameter.Description).Append("</td>");
            }

            builder.Append(CloseTableDataOpenTableRow);
        }

        builder.Append("</table>");
        return builder.ToString();
    }
}
