using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Models
{
    public class Json
    {
        [JsonProperty("client")]
        public JsonClient Client { get; set; }

        [JsonProperty("server")]
        public JsonServer Server { get; set; }
    }

    public class JsonClient
    {
        [JsonProperty("host")]
        public string Host { get; set; }
    }

    public class JsonServer
    {
        [JsonProperty("current_time")]
        public long CurrentTime { get; set; }

        [JsonProperty("services")]
        public List<JsonService> Services { get; set; }
    }

    public class JsonService
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("widgets")]
        public List<JsonWidget> Widgets { get; set; }
    }

    public class JsonWidget
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("params")]
        public List<JsonParam> Params { get; set; }
    }

    public class JsonParam
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
