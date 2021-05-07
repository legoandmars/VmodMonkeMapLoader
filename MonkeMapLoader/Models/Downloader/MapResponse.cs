using Newtonsoft.Json;

namespace VmodMonkeMapLoader.Models.Downloader
{
    public class MapResponse
    {
        [JsonProperty(PropertyName = "isSuccess")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "data")]
        public MapData Data { get; set; }

    }
    public class MapData
    {
        [JsonProperty(PropertyName = "maps")]
        public OnlineMapInfo[] Maps { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
    }
}
