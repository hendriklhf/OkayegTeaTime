#nullable disable

namespace OkayegTeaTime.Files.Models;

public class UserLists
{
    public long Owner { get; set; }

    public long[] Moderators { get; set; }

    public long[] IgnoredUsers { get; set; }

    public long[] SecretUsers { get; set; }
}
