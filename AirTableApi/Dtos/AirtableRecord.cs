using Newtonsoft.Json;

namespace AirTableApi.Dtos
{
    public class AirtableRecord
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("fields")]
        public Dictionary<string, object> Fields { get; set; }

        public DateTime? CreatedTime { get; set; }
    }
}
