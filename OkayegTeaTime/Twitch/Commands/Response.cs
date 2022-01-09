using System.Text;

namespace OkayegTeaTime.Twitch.Commands;

public class Response
{
    public string Message => GetMessage();

    private readonly string _value = string.Empty;
    private readonly StringBuilder? _builder;

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
        if (_builder is null)
        {
            return _value;
        }
        else
        {
            return _builder.ToString();
        }
    }

    public static Response operator +(Response left, string right)
    {
        StringBuilder builder = new(left.Message);
        return new(builder.Append(right));
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
