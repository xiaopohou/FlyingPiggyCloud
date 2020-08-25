using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System.Dynamic;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
    public sealed partial class FileSystem : SixCloudMethodBase
    {
        /// <summary>
        /// 新建文件夹，Parent和Path参数不可共存
        /// </summary>
        /// <param name="Name">新文件夹的名字，如果其他参数为空则在根目录创建此文件夹</param>
        /// <param name="Parent">在UUID为此的文件夹内创建新文件夹</param>
        /// <param name="Path">在此路径下创建新文件夹，如果Name参数为空则创建此路径</param>
        /// <returns></returns>
        public static async Task<FileMetaData> CreatDirectory(string name = "", string parent = "", string path = "")
        {
            dynamic data = new ExpandoObject();
            if (!string.IsNullOrEmpty(name))
            {
                data.name = name;
            }

            if (!string.IsNullOrEmpty(parent))
            {
                data.parent = parent;
            }
            else if (!string.IsNullOrEmpty(path))
            {
                data.path = path;
            }

            return await PostAsync<FileMetaData>(JsonConvert.SerializeObject(data), "/v3/newfile/");
        }

        /// <summary>
        /// 获取指定对象的MetaData
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static async Task<FileMetaData> GetDetailsByIdentity(string identity)
        {
            return await GetAsync<FileMetaData>($"/v3/newfile/{identity}");
        }

        /// <summary>
        /// 获取目录
        /// </summary>
        /// <param name="parentIdentity">该文件夹的ID</param>
        /// <param name="parentPath">路径</param>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <param name="type"></param>
        /// <param name="search"></param>
        /// <param name="hidden"></param>
        /// <param name="directory"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static async Task<FileList> GetDirectory(string parentIdentity = "",
                                                        string parentPath = "",
                                                        int skip = 0,
                                                        int limit = 20,
                                                        Type? type = null,
                                                        string search = "",
                                                        object hidden = null,
                                                        bool? directory = null,
                                                        object label = null)
        {
            dynamic data = new ExpandoObject();
            if (!string.IsNullOrEmpty(parentIdentity))
            {
                data.parentIdentity = parentIdentity;
            }
            else if (!string.IsNullOrEmpty(parentPath))
            {
                data.parentPath = parentPath;
            }
            else
            {
                data.parentPath = "/";
            }

            if (type != null)
            {
                data.type = type;
            }

            data.skip = skip;
            data.limit = limit;
            return await PostAsync<FileList>(JsonConvert.SerializeObject(data), "/v3/newfiles/");
        }

        /// <summary>
        /// 获取下载地址
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static async Task<FileMetaData> GetDownloadUrlByIdentity(string identity)
        {
            return await PostAsync<FileMetaData>(JsonConvert.SerializeObject(new { identity }), "/v3/newfile/download");
        }

        /// <summary>
        /// 重命名文件夹或文件
        /// </summary>
        /// <param name="identity">被重命名的项目</param>
        /// <param name="name">新名称</param>
        /// <returns></returns>
        public static async Task<FileSystemOperate> Rename(string identity, string name)
        {
            var data = new { identity, name };
            return await PostAsync<FileSystemOperate>(JsonConvert.SerializeObject(data), "/v3/newfile/rename");
        }

        /// <summary>
        /// 移动一组文件或文件夹到同一个目录
        /// </summary>
        /// <param name="sourceUUIDList"></param>
        /// <param name="targetDirectoryUUID"></param>
        /// <returns></returns>
        public static async Task<FileSystemOperate> Move(string[] sourceUUIDList, string targetDirectoryUUID)
        {
            var data = new
            {
                sourceIdentity = sourceUUIDList,
                identity = targetDirectoryUUID
            };
            return await PostAsync<FileSystemOperate>(JsonConvert.SerializeObject(data), "/v3/newfile/move");
        }

        /// <summary>
        /// 复制一组文件夹或文件
        /// </summary>
        /// <param name="sourceUUIDList"></param>
        /// <param name="targetDirectoryUUID"></param>
        /// <returns></returns>
        public static async Task<FileSystemOperate> Copy(string[] sourceIdentity, string identity)
        {
            var data = new
            {
                sourceIdentity,
                identity
            };
            return await PostAsync<FileSystemOperate>(JsonConvert.SerializeObject(data), "/v3/newfile/copy");
        }

        /// <summary>
        /// 删除文件夹或文件
        /// </summary>
        /// <param name="uuid">被删除的项目</param>
        /// <returns></returns>
        public static async Task<FileSystemOperate> Remove(string identity)
        {
            return await Remove(new string[] { identity });
        }

        /// <summary>
        /// 删除文件夹或文件
        /// </summary>
        /// <param name="uuid">被删除的项目</param>
        /// <returns></returns>
        public static async Task<FileSystemOperate> Remove(string[] sourceIdentity, bool directDelete = false)
        {
            var data = new { sourceIdentity, directDelete };

            return await PostAsync<FileSystemOperate>(JsonConvert.SerializeObject(data), "/v3/newfile/trash");
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="name">FileMeta的文件名</param>
        /// <param name="parentUUID">上传到哪个文件夹</param>
        /// <param name="hash">哈希</param>
        /// <param name="originalFilename">保存的源信息的文件名</param>
        /// <returns></returns>
        public static async Task<UploadToken> UploadFile(string name,
                                                         string parentUUID = null,
                                                         string parentPath = null,
                                                         string hash = null,
                                                         UploadOption op = UploadOption.Overwrite)
        {
            dynamic data = new ExpandoObject();
            data.name = name;
            data.op = op;

            if (parentUUID != null)
            {
                data.parent = parentUUID;
            }
            else if (parentPath != null)
            {
                data.path = parentPath;
            }
            else
            {
                data.path = "/";
            }

            if (hash != null)
            {
                data.hash = hash;
            }

            dynamic x = await PostAsync<UploadToken>(JsonConvert.SerializeObject(data), "/v3/file/uploadToken");
            return x;
        }
    }
}
