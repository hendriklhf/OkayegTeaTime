using System;

namespace OkayegTeaTime.Resources;

[AttributeUsage(AttributeTargets.Property)]
public class FieldName : Attribute
{
    public string Name { get; }

    public FieldName(string name)
    {
        Name = name;
    }
}
