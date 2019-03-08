using Newtonsoft.Json;

namespace SixCloud.Models
{
    public class PreviewVideoInformation
    {
        [JsonProperty(PropertyName = "clearTexts")]
        public string[] ClearTexts { get; set; }

        [JsonProperty(PropertyName = "clears")]
        public int[] Clears { get; set; }

        [JsonProperty(PropertyName = "encodeKey")]
        public string EncodeKey { get; set; }

        [JsonProperty(PropertyName = "preview")]
        public ObservableCollection<Dictionary<string, string>> Preview { get; set; }
    }

}
