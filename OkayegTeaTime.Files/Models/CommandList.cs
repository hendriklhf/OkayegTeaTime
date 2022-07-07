namespace OkayegTeaTime.Files.Models;

#nullable disable

public class CommandList
{
    public Command[] Commands { get; set; }

    public AfkCommand[] AfkCommands { get; set; }
}
