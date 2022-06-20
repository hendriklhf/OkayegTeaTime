using System.Text;

namespace OkayegTeaTime.Twitch.Commands;

public class Response
{
    public string Message => GetMessage();

    private readonly string _value = string.Empty;
    private StringBuilder? _builder;

    public Response()
    {
    }

    public Response(string init)
    {
        _value = init;
    }

    private Response(StringBuilder builder)
    {
        _builder = builder;
    }

    private string GetMessage()
    {
        return _builder is null ? _value : _builder.ToString();
    }

    public static Response operator +(Response left, string right)
    {
        left._builder ??= new(left.Message);
        return new(left._builder.Append(right));
    }

    public static Response operator +(Response left, char right)
    {
        left._builder ??= new(left.Message);
        return new(left._builder.Append(right));
    }

    public static implicit operator Response(string str)
    {
        return new(str);
    }

    public override bool Equals(object? obj)
    {
        return obj is Response res && res.Message == Message;
    }

    public override string ToString()
    {
        return Message;
    }
}
