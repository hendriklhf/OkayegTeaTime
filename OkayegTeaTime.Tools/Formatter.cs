using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Tools;

public class Formatter
{
    private static readonly List<string> _files = new();
    private static readonly Regex _filePattern = NewRegex(@"\.sql$");
    private static readonly Regex _commentPattern = NewRegex("^--");
    private static readonly Regex _anyWordCharPattern = NewRegex(@"\w+");

    private const string _folder = "./OkayegTeaTime/Resources/";

    public Formatter()
    {
        FindSqlFiles(_folder);
    }

    private void FindSqlFiles(string path)
    {
        string[] directories = Directory.GetDirectories(path);
        foreach (string dir in directories)
        {
            FindSqlFiles(dir);
        }
        string[] files = Directory.GetFiles(path).Where(f => _filePattern.IsMatch(f)).ToArray();
        _files.AddRange(files);
    }

    public void FormatFiles()
    {
        foreach (string file in _files)
        {
            string[] lines = File.ReadAllLines(file);
            lines = lines.Where(l => !_commentPattern.IsMatch(l) && _anyWordCharPattern.IsMatch(l)).ToArray();
            File.WriteAllLines(file, lines);
        }
    }
}
