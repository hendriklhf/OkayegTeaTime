using System;
using System.Diagnostics.CodeAnalysis;

namespace OkayegTeaTime.Database.EntityFrameworkModels;

public sealed class Suggestion
{
    public int Id { get; set; }

    public string Username { get; set; }

    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public byte[] Content { get; set; }

    public string Channel { get; set; }

    [SuppressMessage("Major Code Smell", "S6354:Use a testable date/time provider")]
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
        if (channel[0] == '#')
        {
            channel = channel[1..];
        }

        Channel = '#' + channel;
    }
}
