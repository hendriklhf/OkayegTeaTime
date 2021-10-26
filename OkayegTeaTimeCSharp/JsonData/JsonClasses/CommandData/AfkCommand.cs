namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData;

public class AfkCommand
{
    public string CommandName { get; set; }

    public string ComingBack { get; set; }

    public string GoingAway { get; set; }

    public string Resuming { get; set; }

    public List<string> Alias { get; set; }

    public List<string> Parameter { get; set; }

    public List<string> Description { get; set; }
}
