#nullable disable

namespace OkayegTeaTime.Files.Jsons.CommandData;

public class Command
{
    public string Name { get; set; }

    public string[] Alias { get; set; }

    public string[] Parameter { get; set; }

    public string[] Description { get; set; }

    public int Cooldown { get; set; }

    public bool Document { get; set; }
}
