using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Models.Dashboard.CatAPI
{
    public class Cat
    {
        public partial class CatJson
        {
            [JsonProperty("breeds")]
            public object[] Breeds { get; set; }

            [JsonProperty("categories")]
            public Category[] Categories { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("url")]
            public Uri Url { get; set; }

            [JsonProperty("width")]
            public long Width { get; set; }

            [JsonProperty("height")]
            public long Height { get; set; }
        }

        public partial class Category
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }
    }
}
