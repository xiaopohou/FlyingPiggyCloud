using Newtonsoft.Json;
using System.Collections.Generic;

namespace QingzhenyunApis.EntityModels
{
    public class FileList : EntityBodyBase
    {
        /// <summary>
        /// 文件夹及文件列表
        /// </summary>
        [JsonProperty(PropertyName = "dataList")]
        public List<FileMetaData> List { get; set; }

        /// <summary>
        /// 当前目录MetaData
        /// </summary>
        [JsonProperty(PropertyName = "parent")]
        public FileMetaData DictionaryInformation { get; set; }

    }
}
