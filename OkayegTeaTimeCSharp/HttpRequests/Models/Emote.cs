namespace OkayegTeaTimeCSharp.HttpRequests.Models
{
    public class Emote
    {
        public int Index { get; }

        public string Name { get; }

        public Emote(int index, string name)
        {
            Index = index;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}