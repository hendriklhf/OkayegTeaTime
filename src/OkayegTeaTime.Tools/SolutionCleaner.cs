using System;
using System.IO;
using System.Linq;

namespace OkayegTeaTime.Tools;

internal sealed class SolutionCleaner
{
    private readonly string[] _directoriesToClean =
    [
        "Build/",
        "OkayegTeaTime/bin/",
        "OkayegTeaTime.Database/bin/",
        "OkayegTeaTime.Configuration/bin/",
        "OkayegTeaTime.Resources/bin/",
        "OkayegTeaTime.Spotify/bin/",
        "OkayegTeaTime.Tests/bin/",
        "OkayegTeaTime.Tools/bin/",
        "OkayegTeaTime.Twitch/bin/",
        "OkayegTeaTime.Utils/bin/",
        "TestResults/"
    ];

    public void Clean()
    {
        foreach (string directory in _directoriesToClean.Where(Directory.Exists))
        {
            try
            {
                Directory.Delete(directory, true);
                Console.WriteLine($"Deleted \"{directory}\"");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to delete \"{directory}\" => {ex.GetType()}: {ex.Message}");
            }
        }
    }
}
