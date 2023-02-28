namespace OkayegTeaTime.Models.Formula1;

public sealed class Session
{
    public string Name { get; }

    public DateTime Start { get; }

    public Session(string name, DateTime start)
    {
        Name = name;
        Start = start;
    }
}
