using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class PreviewInformation
    {
        /// <summary>
        /// 高度
        /// </summary>
        [JsonProperty(PropertyName = "heigth")]
        public int Heigth { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }

        [JsonProperty(PropertyName ="rotate")]
        public int Rotate { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }

        [JsonProperty(PropertyName ="sourceWidth")]
        public int SourceWidth { get; set; }

        [JsonProperty(PropertyName ="sourceHeight")]
        public int SourceHeight { get; set; }


        [JsonProperty(PropertyName = "title")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "playAddress")]
        public string PreviewHlsAddress { get; set; }


        //[JsonProperty(PropertyName = "sourceIdentity")]
        //public string SoutceIdentity { get; set; }

        //[JsonProperty(PropertyName = "sourcePath")]
        //public string SourcePath { get; set; }

        //[JsonProperty(PropertyName = "sourceSize")]
        //public int SourceSize { get; set; }

        //[JsonProperty(PropertyName = "previewImageAddress")]
        //public string[] PreviewImageAddress { get; set; }


        //[JsonProperty(PropertyName = "status")]
        //public int Status { get; set; }

    }
}
