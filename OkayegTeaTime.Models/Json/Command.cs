#nullable disable

namespace OkayegTeaTime.Models.Json;

public sealed class Command
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string[] Aliases { get; set; }

    public string[] Parameters { get; set; }

    public string[] ParameterDescriptions { get; set; }

    public int Cooldown { get; set; }

    public bool Document { get; set; }
}
