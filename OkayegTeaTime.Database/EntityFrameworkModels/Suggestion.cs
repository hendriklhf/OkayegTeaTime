using System;
using HLE;

#nullable disable

namespace OkayegTeaTime.Database.EntityFrameworkModels;

public sealed class Suggestion
{
    public int Id { get; set; }

    public string Username { get; set; }

    public byte[] Content { get; set; }

    public string Channel { get; set; }

    public long Time { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public string Status { get; set; } = "Open";

    public Suggestion(int id, string username, byte[] content, string channel, long time, string status)
    {
        Id = id;
        Username = username;
        Content = content;
        Channel = channel;
        Time = time;
        Status = status;
    }

    public Suggestion(string username, byte[] suggestion, string channel)
    {
        Username = username;
        Content = suggestion;
        Channel = $"#{channel.Remove("#")}";
    }
}
