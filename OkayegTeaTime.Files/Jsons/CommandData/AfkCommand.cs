#nullable disable

namespace OkayegTeaTime.Files.Jsons.CommandData;

public class AfkCommand
{
    public string Name { get; set; }

    public string ComingBack { get; set; }

    public string GoingAway { get; set; }

    public string Resuming { get; set; }

    public string[] Alias { get; set; }

    public string[] Parameter { get; set; }

    public string[] Description { get; set; }

    public bool Document { get; set; }
}
