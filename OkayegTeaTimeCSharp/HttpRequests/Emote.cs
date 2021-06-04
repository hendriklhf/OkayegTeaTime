namespace OkayegTeaTimeCSharp.HttpRequests
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
    }
}