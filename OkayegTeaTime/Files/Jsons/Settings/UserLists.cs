#nullable disable

namespace OkayegTeaTime.Files.Jsons.Settings;

public class UserLists
{
    public List<long> Owners { get; set; }

    public List<long> Moderators { get; set; }

    public List<long> IgnoredUsers { get; set; }

    public List<long> SecretUsers { get; set; }
}
