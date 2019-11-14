using Newtonsoft.Json;

namespace SixCloud.Models
{
    public class PreviewVideoInformation
    {
        //[JsonProperty(PropertyName = "clearTexts")]
        //public string[] ClearTexts { get; set; }

        //[JsonProperty(PropertyName = "clears")]
        //public int[] Clears { get; set; }

        //[JsonProperty(PropertyName = "encodeKey")]
        //public string EncodeKey { get; set; }

        //[JsonProperty(PropertyName = "preview")]
        //public ResourceInformation[] Preview { get; set; }

        //public class ResourceInformation
        //{
        //    [JsonProperty(PropertyName = "clear")]
        //    public int Clear { get; set; }

        //    [JsonProperty(PropertyName = "duration")]
        //    public int Duration { get; set; }

        //    [JsonProperty(PropertyName = "resolution")]
        //    public string Resolution { get; set; }

        //    [JsonProperty(PropertyName = "clearText")]
        //    public string ClearText { get; set; }

        //    [JsonProperty(PropertyName = "url")]
        //    public string Url { get; set; }
        //}

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }

        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }

        [JsonProperty(PropertyName = "heigth")]
        public int Heigth { get; set; }

        [JsonProperty(PropertyName = "sourceIdentity")]
        public string SoutceIdentity { get; set; }

        [JsonProperty(PropertyName = "sourcePath")]
        public string SourcePath { get; set; }

        [JsonProperty(PropertyName = "sourceSize")]
        public int SourceSize { get; set; }

        [JsonProperty(PropertyName = "previewImageAddress")]
        public string[] PreviewImageAddress { get; set; }

        [JsonProperty(PropertyName = "previewHlsAddress")]
        public string PreviewHlsAddress { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

    }

}
