﻿using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class RecoveryBoxItem : EntityBodyBase
    {
        [JsonProperty(PropertyName = "identity")]
        public string Identity { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        [JsonProperty(PropertyName = "deleteTime")]
        public long DeleteTime { get; set; }

        [JsonProperty(PropertyName = "directory")]
        public bool Directory { get; set; }
    }
}
