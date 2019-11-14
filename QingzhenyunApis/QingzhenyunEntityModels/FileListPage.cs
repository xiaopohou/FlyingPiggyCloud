using Newtonsoft.Json;

namespace SixCloud.Models
{
    public class FileListPage
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        [JsonProperty(PropertyName = "page")]
        public int Page { get; set; }

        /// <summary>
        /// 每页展示的文件数，Max：999
        /// </summary>
        [JsonProperty(PropertyName = "pageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// 文件总数
        /// </summary>
        [JsonProperty(PropertyName = "totalCount")]
        public int TotalCount { get; set; }

        /// <summary>
        /// 页总数
        /// </summary>
        [JsonProperty(PropertyName = "totalPage")]
        public int TotalPage { get; set; }

        /// <summary>
        /// 本页目录及文件列表
        /// </summary>
        [JsonProperty(PropertyName = "list")]
        public FileMetaData[] List { get; set; }

        /// <summary>
        /// 当前目录MetaData
        /// </summary>
        [JsonProperty(PropertyName = "parent")]
        public FileMetaData DictionaryInformation { get; set; }
    }
}
