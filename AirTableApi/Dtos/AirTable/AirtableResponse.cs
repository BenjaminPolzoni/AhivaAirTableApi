using Newtonsoft.Json;

namespace AirTableApi.Dtos.AirTable
{
    public class AirtableResponse
    {
        [JsonProperty("records")]
        public List<AirtableRecord> Records { get; set; }

        [JsonProperty("offset")]
        public string Offset { get; set; }   // ⬅️ ESTA ES LA QUE FALTABA
    }
}
