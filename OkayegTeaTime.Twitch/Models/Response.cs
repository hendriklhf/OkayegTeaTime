using System.Text;

namespace OkayegTeaTime.Twitch.Models;

public sealed class Response
{
    public static Response Empty { get; } = new(string.Empty);

    private readonly string _value;
    private StringBuilder? _builder;

    private Response(string init)
    {
        _value = init;
    }

    private Response(StringBuilder builder)
    {
        _value = string.Empty;
        _builder = builder;
    }

    private string GetMessage()
    {
        return _builder is null ? _value : _builder.ToString();
    }

    public static Response operator +(Response left, string right)
    {
        if (left == Empty)
        {
            left = new(string.Empty);
        }

        left._builder ??= new(left.GetMessage());
        return new(left._builder.Append(right));
    }

    public static Response operator +(Response left, char right)
    {
        if (left == Empty)
        {
            left = new(string.Empty);
        }

        left._builder ??= new(left.GetMessage());
        return new(left._builder.Append(right));
    }

    public static implicit operator Response(string str)
    {
        return new(str);
    }

    public static implicit operator string(Response response)
    {
        return response.GetMessage();
    }

    public override bool Equals(object? obj)
    {
        return obj is Response res && res.GetMessage() == GetMessage();
    }

    public override int GetHashCode()
    {
        return GetMessage().GetHashCode();
    }

    public override string ToString()
    {
        return GetMessage();
    }
}
