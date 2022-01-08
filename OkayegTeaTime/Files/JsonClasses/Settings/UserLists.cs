#nullable disable

namespace OkayegTeaTime.Files.JsonClasses.Settings;

public class UserLists
{
    public List<int> Owners { get; set; }

    public List<int> Moderators { get; set; }

    public List<int> IgnoredUsers { get; set; }

    public List<int> SecretUsers { get; set; }
}
