using Newtonsoft.Json;
using System;

namespace QingzhenyunApis.EntityModels
{
    public class ScreenshotInformation
    {
        [JsonProperty("found")]
        public bool Found { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }

        [JsonProperty("downloadAddress")]
        public Uri DownloadAddress { get; set; }
    }
}
