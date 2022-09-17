namespace OkayegTeaTime.Files.Models;

#nullable disable

public sealed class CommandList
{
    public Command[] Commands { get; set; }

    public AfkCommand[] AfkCommands { get; set; }
}
