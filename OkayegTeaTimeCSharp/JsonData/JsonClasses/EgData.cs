using Newtonsoft.Json;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class EgData
    {
        [JsonProperty("MaxLevel")]
        public int MaxLevel { get; set; }

        [JsonProperty("Possibilities")]
        public List<int> Possibilites { get; set; }
    }
}
