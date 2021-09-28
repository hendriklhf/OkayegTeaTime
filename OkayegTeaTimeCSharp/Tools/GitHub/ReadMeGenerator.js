const file = require("fs");
const jsonSerializer = require("../../../../JsonSerializerJS/JsonSerializer");

// Data
const commands = jsonSerializer.deserialize("../../Resources/Commands.json");
const readMePath = "../../../README.md";
const title = "OkayegTeaTime";
const header1 = "Commands";
const header2 = "AFK-Commands";
const header1Text = [
    "If your channel has a custom prefix set, commands will have to start with the prefix.",
    "If your channel has no prefix set, commands will have to end with \"eg\", for example: \"pingeg\".",
    "Text in \"[ ]\" is a variable parameter"
];
const cmdTableHeader = [
    "Command",
    "Alias",
    "Description [Parameter | Output]"
];
const afkCmdTableHeader = [
    "Command",
    "Alias",
    "Parameter",
    "Description"
];
const linebreak = "<br />";
// Data

main();

function main() {
    generateReadMe();
}

function generateReadMe() {
    try {
        file.writeFileSync(readMePath, generateString());
        console.log("Generated README.md");
    } catch (err) {
        console.error(err);
    }
}

function generateString() {
    var result = "";
    result += `<h1>${title}</h1><h2>${header1}</h2>`;
    header1Text.forEach(h => result += h + linebreak);
    result += `${linebreak}<table><tr>`;
    cmdTableHeader.forEach(c => result += `<th>${c}</th>`);
    result += "</tr>";
    commands.Commands.sort((c1, c2) => sortCmd(c1, c2)).forEach(cmd => {
        result += `<tr><td>${cmd.CommandName}</td><td><table>`;
        cmd.Alias.forEach(a => result += `<tr><td>${a}</td></tr>`);
        result += "</table></td><td><table>";
        for (var i = 0; i <= cmd.Parameter.length - 1; i++) {
            result += `<tr><td>${cmd.Parameter[i]}</td><td>${cmd.Description[i]}</td></tr>`;
        }
        result += "</table></td></tr>";
    });
    result += `</table><h2>${header2}</h2><table><tr>`;
    afkCmdTableHeader.sort((c1, c2) => sortCmd(c1, c2)).forEach(a => result += `<th>${a}</th>`);
    result += "</tr>";
    commands.AfkCommands.forEach(cmd => {
        result += `<tr><td>${cmd.CommandName}</td><td><table>`;
        cmd.Alias.forEach(a => result += `<tr><td>${a}</td></tr>`);
        result += "</table></td>";
        for (var i = 0; i <= cmd.Parameter.length - 1; i++) {
            result += `<td>${cmd.Parameter[i]}</td><td>${cmd.Description[i]}</td>`;
        }
        result += "</td></tr>";
    });
    result += "</table>";
    return result;
}

function sortCmd(cmdOne, cmdTwo) {
    if (cmdOne.CommandName < cmdTwo.CommandName) {
        return -1;
    } else if (cmdOne.CommandName < cmdTwo.CommandName) {
        return 1;
    } else {
        return 0;
    }
}
