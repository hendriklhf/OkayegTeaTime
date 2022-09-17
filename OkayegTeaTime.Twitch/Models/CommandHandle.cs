using System.Reflection;

namespace OkayegTeaTime.Twitch.Models;

public sealed class CommandHandle
{
    public ConstructorInfo Constructor { get; set; }

    public MethodInfo HandleMethod { get; set; }

    public MethodInfo SendMethod { get; set; }

    public CommandHandle(ConstructorInfo constructor, MethodInfo handleMethod, MethodInfo sendMethod)
    {
        Constructor = constructor;
        HandleMethod = handleMethod;
        SendMethod = sendMethod;
    }
}
