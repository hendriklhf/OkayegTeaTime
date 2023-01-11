using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace OkayegTeaTime.Api.Controllers;

[ApiController]
[Route("api/cmds")]
public sealed class CommandController : ControllerBase
{
    private readonly Twitch.Controller.CommandController _commandController = new(null);

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    private string? _commands;
    private string? _afkCommmands;

    [HttpGet("commands")]
    public string GetCommands()
    {
        Response.Headers.Add("Access-Control-Allow-Origin", "*");
        return _commands ??= JsonSerializer.Serialize(_commandController.Commands, _options);
    }

    [HttpGet("afkcommands")]
    public string GetAfkCommands()
    {
        Response.Headers.Add("Access-Control-Allow-Origin", "*");
        return _afkCommmands ??= JsonSerializer.Serialize(_commandController.AfkCommands, _options);
    }
}
