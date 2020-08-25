using Newtonsoft.Json;
using System.Collections.Generic;

namespace QingzhenyunApis.EntityModels
{
    public class FileList : DataListResult<FileMetaData>
    {
        /// <summary>
        /// 当前目录MetaData
        /// </summary>
        [JsonProperty(PropertyName = "parent")]
        public FileMetaData DictionaryInformation { get; set; }

    }

}
