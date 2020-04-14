using Newtonsoft.Json;
using System.Collections.Generic;

namespace QingzhenyunApis.EntityModels
{
    //public class FileListPage : FileList
    //{
    //    [JsonProperty(PropertyName = "pageInfo")]
    //    public FileListPageInfo FileListPageInfo { get; set; }
    //}

    //public class FileListPageInfo
    //{
    //    /// <summary>
    //    /// 当前页码
    //    /// </summary>
    //    [JsonProperty(PropertyName = "page")]
    //    public int Page { get; set; }

    //    /// <summary>
    //    /// 每页展示的文件数，Max：999
    //    /// </summary>
    //    [JsonProperty(PropertyName = "pageSize")]
    //    public int PageSize { get; set; }

    //    /// <summary>
    //    /// 文件总数
    //    /// </summary>
    //    [JsonProperty(PropertyName = "totalRecord")]
    //    public int TotalCount { get; set; }

    //    /// <summary>
    //    /// 页总数
    //    /// </summary>
    //    [JsonProperty(PropertyName = "totalPage")]
    //    public int TotalPage { get; set; }


    //}

    public class FileList
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

    public class SuccessCount
    {
        [JsonProperty(PropertyName = "successCount")]
        public int Value { get; set; }
    }
}
