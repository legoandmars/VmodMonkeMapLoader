using Newtonsoft.Json;

namespace VmodMonkeMapLoader.Models.Downloader
{
    public class OnlineMapInfo
    {
        [JsonProperty(PropertyName = "mapId")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "mapGuid")]
        public string GUID { get; set; }

        [JsonProperty(PropertyName = "mapVersion")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "mapName")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "mapFileUrl")]
        public string FileURL { get; set; }

        [JsonProperty(PropertyName = "mapThumbnailFileUrl")]
        public string ThumbnailFileURL { get; set; }

        [JsonProperty(PropertyName = "mapDateAdded")]
        public string DateAdded { get; set; }

        [JsonProperty(PropertyName = "mapDateUpdated")]
        public string DateUpdated { get; set; }

        [JsonProperty(PropertyName = "mapDownloadCount")]
        public int Downloads { get; set; }

        [JsonProperty(PropertyName = "mapRating")]
        public float Rating { get; set; }

        [JsonProperty(PropertyName = "mapRatingVotesCount")]
        public int RatingVotesCount { get; set; }

        [JsonProperty(PropertyName = "mapFileName")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "mapFileSize")]
        public int FileSize { get; set; }

        [JsonProperty(PropertyName = "authorDiscordId")]
        public string AuthorID { get; set; }

        [JsonProperty(PropertyName = "authorName")]
        public string AuthorName { get; set; }

        [JsonProperty(PropertyName = "authorDiscriminator")]
        public string AuthorDiscriminator { get; set; }

        [JsonProperty(PropertyName = "userVoted")]
        public bool UserVoted { get; set; }

        [JsonProperty(PropertyName = "userRatingValue")]
        public float UserRatingValue { get; set; }
    }
}
