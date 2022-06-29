#nullable disable

namespace OkayegTeaTime.Files.Jsons.CommandData;

public class AfkCommand
{
    public string Name { get; set; }

    public string ComingBack { get; set; }

    public string GoingAway { get; set; }

    public string Resuming { get; set; }

    public string[] Aliases { get; set; }

    public string[] Parameters { get; set; }

    public string[] ParameterDescriptions { get; set; }

    public bool Document { get; set; }
}
