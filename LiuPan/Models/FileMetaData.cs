using Newtonsoft.Json;

namespace LiuPan.Models
{
    public class FileMetaData
    {
        /// <summary>
        /// 该文件/文件夹的唯一ID
        /// </summary>
        [JsonProperty(PropertyName = "uuid")]
        public string UUID { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 文件mime，即文件的类型
        /// </summary>
        [JsonProperty(PropertyName = "mime")]
        public string Mime { get; set; }

        /// <summary>
        /// 0:为文件,1:目录
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public int Type { get; set; }

        /// <summary>
        /// 父目录id
        /// </summary>
        [JsonProperty(PropertyName = "parent")]
        public string Parent { get; set; }

        /// <summary>
        /// 文件创建时间
        /// </summary>
        [JsonProperty(PropertyName = "ctime")]
        public long Ctime { get; set; }

        /// <summary>
        /// 文件修改时间
        /// </summary>
        [JsonProperty(PropertyName = "mtime")]
        public long Mtime { get; set; }

        /// <summary>
        /// 文件访问时间
        /// </summary>
        [JsonProperty(PropertyName = "atime")]
        public long Atime { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [JsonProperty(PropertyName = "userId")]
        public long UserId { get; set; }


        /// <summary>
        /// 路径
        /// </summary>
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        /// <summary>
        /// 该文件或文件夹的访问路径
        /// </summary>
        [JsonProperty(PropertyName = "ext")]
        public string Ext { get; set; }

        /// <summary>
        /// 文件大小(字节)
        /// </summary>
        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        /// <summary>
        /// 总是 0
        /// </summary>
        [JsonProperty(PropertyName = "flag")]
        public int Flag { get; set; }

        /// <summary>
        /// 预览状态
        /// </summary>
        [JsonProperty(PropertyName = "preview")]
        public int Preview { get; set; }

        /// <summary>
        /// 0:正常文件，1:回收站
        /// </summary>
        [JsonProperty(PropertyName = "recycle")]
        public int Recycle { get; set; }

        /// <summary>
        /// 文件版本
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        [JsonProperty(PropertyName = "locking")]
        public bool Locking { get; set; }

        /// <summary>
        /// 如果锁定,文件锁定的情况下，当前后台正进行的操作
        /// </summary>
        [JsonProperty(PropertyName = "opt")]
        public int Opt { get; set; }

        /// <summary>
        /// 文件下载地址
        /// </summary>
        [JsonProperty(PropertyName = "downloadAddress")]
        public string DownloadAddress { get; set; }

    }
}
