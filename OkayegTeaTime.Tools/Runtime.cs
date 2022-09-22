namespace OkayegTeaTime.Tools;

public sealed record Runtime
{
    public required string Name { get; init; }

    public required string Identifier { get; init; }

    public static Runtime Windows64Bit { get; } = new()
    {
        Name = "Windows 64-bit",
        Identifier = "win-x64"
    };

    public static Runtime Linux64Bit { get; } = new()
    {
        Name = "Linux 64-bit",
        Identifier = "linux-x64"
    };

    public static Runtime LinuxArm { get; } = new()
    {
        Name = "Linux Arm",
        Identifier = "linux-arm"
    };

    public static Runtime MacOs64Bit { get; } = new()
    {
        Name = "macOS 64-bit",
        Identifier = "osx-x64"
    };
}
