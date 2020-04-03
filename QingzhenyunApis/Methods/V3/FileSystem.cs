using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
    public sealed class FileSystem : SixCloudMethodBase
    {
        /// <summary>
        /// 新建文件夹，Parent和Path参数不可共存
        /// </summary>
        /// <param name="Name">新文件夹的名字，如果其他参数为空则在根目录创建此文件夹</param>
        /// <param name="Parent">在UUID为此的文件夹内创建新文件夹</param>
        /// <param name="Path">在此路径下创建新文件夹，如果Name参数为空则创建此路径</param>
        /// <returns></returns>
        public static async Task<GenericResult<FileMetaData>> CreatDirectory(string name = "", string parent = "", string path = "")
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

            return await PostAsync<GenericResult<FileMetaData>>(JsonConvert.SerializeObject(data), "/v3/file");
        }

        /// <summary>
        /// 获取指定对象的MetaData
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static async Task<GenericResult<FileMetaData>> GetDetailsByIdentity(string identity)
        {
            return await GetAsync<FileMetaData>($"/v3/file/{identity}");
        }

        /// <summary>
        /// 获取目录
        /// </summary>
        /// <param name="parentIdentity">该文件夹的ID</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static async Task<GenericResult<FileListPage>> GetDirectory(string parentIdentity = "", string path = "")
        {
            dynamic data = new ExpandoObject();
            if (!string.IsNullOrEmpty(parentIdentity))
            {
                data.parentIdentity = parentIdentity;
            }
            else if (!string.IsNullOrEmpty(path))
            {
                data.parentPath = path;
            }
            else
            {
                data.parentPath = "/";
            }
            return await PostAsync<GenericResult<FileList>>(JsonConvert.SerializeObject(data), "/v3/files/list");
        }

        /// <summary>
        /// 以分页的方式获取目录
        /// </summary>
        /// <param name="parent">该文件夹的ID</param>
        /// <param name="path">路径</param>
        /// <param name="page">第几页</param>
        /// <param name="pageSize">列表大小，最大值999</param>
        /// <param name="OrderBy">排序：0按文件名，1按时间</param>
        /// <param name="Type">文件类型：0显示文件，1显示文件夹，-1显示文件和文件夹(默认)</param>
        /// <returns></returns>
        public static async Task<GenericResult<FileListPage>> GetDirectoryAsPage(string parentIdentity = "", string path = "", int page = 0, int pageSize = 20, string[,] orderBy = null)
        {
            dynamic data = new ExpandoObject();
            if (!string.IsNullOrEmpty(parentIdentity))
            {
                data.parentIdentity = parentIdentity;
            }
            else if (!string.IsNullOrEmpty(path))
            {
                data.parentPath = path;
            }
            else
            {
                data.parentPath = "/";
            }

            data.page = page;
            data.pageSize = pageSize;
            if (orderBy != null)
            {
                data.orderBy = orderBy;
            }

            return await PostAsync<GenericResult<FileListPage>>(JsonConvert.SerializeObject(data), "/v3/files/page");
        }

        /// <summary>
        /// 获取下载地址
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static async Task<GenericResult<FileMetaData>> GetDownloadUrlByIdentity(string identity)
        {
            return await PostAsync<FileMetaData>(JsonConvert.SerializeObject(new { identity }), "/v3/file/download");
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="name">FileMeta的文件名</param>
        /// <param name="parentUUID">上传到哪个文件夹</param>
        /// <param name="hash">哈希</param>
        /// <param name="originalFilename">保存的源信息的文件名</param>
        /// <returns></returns>
        public static async Task<GenericResult<UploadToken>> UploadFile(string name, string parentUUID = null, string parentPath = null, string hash = null, string originalFilename = null)
        {
            dynamic data = new ExpandoObject();
            data.name = name;

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

            return await PostAsync<GenericResult<UploadToken>>(JsonConvert.SerializeObject(data), "/v3/file/uploadToken");
        }

        /// <summary>
        /// 移动文件夹或文件
        /// </summary>
        /// <param name="SourceMeta">被移动的项目</param>
        /// <param name="TargetDirectory">目标位置，必须是一个文件夹的Meta信息</param>
        /// <returns></returns>
        public static async Task<GenericResult<int?>> Move(string sourceUUID, string targetDirectoryUUID)
        {
            return await Move(new string[] { sourceUUID }, targetDirectoryUUID);
        }

        /// <summary>
        /// 移动一组文件或文件夹到同一个目录
        /// </summary>
        /// <param name="sourceUUIDList"></param>
        /// <param name="targetDirectoryUUID"></param>
        /// <returns></returns>
        public static async Task<GenericResult<int?>> Move(string[] sourceUUIDList, string targetDirectoryUUID)
        {
            var data = new
            {
                sourceIdentity = sourceUUIDList,
                identity = targetDirectoryUUID
            };
            return await PostAsync<int?>(JsonConvert.SerializeObject(data), "/v3/file/move");
        }

        /// <summary>
        /// 复制文件夹或文件
        /// </summary>
        /// <param name="SourceMeta">被复制的项目</param>
        /// <param name="TargetDirectory">目标位置，必须是一个文件夹的Meta信息</param>
        /// <returns></returns>
        public static async Task<GenericResult<int?>> Copy(string sourceUUID, string targetDirectoryUUID)
        {
            return await Copy(new string[] { sourceUUID }, targetDirectoryUUID);
        }

        /// <summary>
        /// 复制一组文件夹或文件
        /// </summary>
        /// <param name="sourceUUIDList"></param>
        /// <param name="targetDirectoryUUID"></param>
        /// <returns></returns>
        public static async Task<GenericResult<int?>> Copy(string[] sourceUUIDList, string targetDirectoryUUID)
        {
            var data = new
            {
                sourceIdentity = sourceUUIDList,
                identity = targetDirectoryUUID
            };
            return await PostAsync<int?>(JsonConvert.SerializeObject(data), "/v3/file/copy");
        }

        /// <summary>
        /// 重命名文件夹或文件
        /// </summary>
        /// <param name="identity">被重命名的项目</param>
        /// <param name="name">新名称</param>
        /// <returns></returns>
        public static async Task<GenericResult<bool>> Rename(string identity, string name)
        {
            var data = new { identity, name };
            return await PostAsync<bool>(JsonConvert.SerializeObject(data), "/v3/file/rename");
        }



#warning 这里的代码还没有写完

        /// <summary>
        /// 删除文件夹或文件
        /// </summary>
        /// <param name="uuid">被删除的项目</param>
        /// <returns></returns>
        public async Task<GenericResult<int?>> Remove(string identity)
        {
            var data = new
            {
                source = new object[] { new { identity } }
            };
            return await PostAsync<int?>(JsonConvert.SerializeObject(data), "/v2/files/delete");
        }

        /// <summary>
        /// 删除文件夹或文件
        /// </summary>
        /// <param name="uuid">被删除的项目</param>
        /// <returns></returns>
        public async Task<GenericResult<int?>> Remove(string[] uuids)
        {
            List<object> list = new List<object>(uuids.Length);
            foreach (string uuid in uuids)
            {
                list.Add(new { identity = uuid });
            }
            var data = new { source = list.ToArray() };

            return await PostAsync<int?>(JsonConvert.SerializeObject(data), "/v2/files/delete");
        }
    }
}
