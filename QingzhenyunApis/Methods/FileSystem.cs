using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods
{
    public sealed class FileSystem : SixCloudMethordBase
    {
        /// <summary>
        /// 请求的时候，如果 name 和 path均为空，则会返回根目录
        /// </summary>
        /// <param name="parent">该文件夹的ID</param>
        /// <param name="path">路径</param>
        /// <param name="page">第几页</param>
        /// <param name="pageSize">列表大小，最大值999</param>
        /// <param name="OrderBy">排序：0按文件名，1按时间</param>
        /// <param name="Type">文件类型：0显示文件，1显示文件夹，-1显示文件和文件夹(默认)</param>
        /// <returns></returns>
        public async Task<GenericResult<FileListPage>> GetDirectory(string parent = "", string path = "", int? page = null, int? pageSize = null, int orderBy = 1)
        {
            dynamic data = new ExpandoObject();
            if (!string.IsNullOrEmpty(parent))
            {
                data.identity = parent;
            }
            else if (!string.IsNullOrEmpty(path))
            {
                data.path = path;
            }

            if (page != null)
            {
                data.page = page;
            }

            if (pageSize != null)
            {
                data.pageSize = pageSize;
            }
            data.orderBy = orderBy;
            return await PostAsync<GenericResult<FileListPage>>(JsonConvert.SerializeObject(data), "/v2/files/page", false);
        }

        /// <summary>
        /// 新建文件夹，Parent和Path参数不可共存
        /// </summary>
        /// <param name="Name">新文件夹的名字，如果其他参数为空则在根目录创建此文件夹</param>
        /// <param name="Parent">在UUID为此的文件夹内创建新文件夹</param>
        /// <param name="Path">在此路径下创建新文件夹，如果Name参数为空则创建此路径</param>
        /// <returns></returns>
        public async Task<GenericResult<FileMetaData>> CreatDirectory(string name = "", string parent = "", string path = "")
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

            return await PostAsync<GenericResult<FileMetaData>>(JsonConvert.SerializeObject(data), "/v2/files/createDirectory", false);
        }

        /// <summary>
        /// 移动文件夹或文件
        /// </summary>
        /// <param name="SourceMeta">被移动的项目</param>
        /// <param name="TargetDirectory">目标位置，必须是一个文件夹的Meta信息</param>
        /// <returns></returns>
        public async Task<GenericResult<int?>> Move(string sourceUUID, string targetDirectoryUUID)
        {
            return await Move(new string[] { sourceUUID }, targetDirectoryUUID);
        }

        /// <summary>
        /// 移动一组文件或文件夹到同一个目录
        /// </summary>
        /// <param name="sourceUUIDList"></param>
        /// <param name="targetDirectoryUUID"></param>
        /// <returns></returns>
        public async Task<GenericResult<int?>> Move(string[] sourceUUIDList, string targetDirectoryUUID)
        {
            List<object> list = new List<object>(sourceUUIDList.Length);
            foreach (string uuid in sourceUUIDList)
            {
                var a = new { identity = uuid };
                list.Add(a);
            }
            var data = new
            {
                source = list,
                destination = new { identity = targetDirectoryUUID }
            };
            return await PostAsync<GenericResult<int?>>(JsonConvert.SerializeObject(data), "/v2/files/move", false);
        }

        /// <summary>
        /// 复制文件夹或文件
        /// </summary>
        /// <param name="SourceMeta">被复制的项目</param>
        /// <param name="TargetDirectory">目标位置，必须是一个文件夹的Meta信息</param>
        /// <returns></returns>
        public async Task<GenericResult<int?>> Copy(string sourceUUID, string targetDirectoryUUID)
        {
            return await Copy(new string[] { sourceUUID }, targetDirectoryUUID);
        }

        /// <summary>
        /// 复制一组文件夹或文件
        /// </summary>
        /// <param name="sourceUUIDList"></param>
        /// <param name="targetDirectoryUUID"></param>
        /// <returns></returns>
        public async Task<GenericResult<int?>> Copy(string[] sourceUUIDList, string targetDirectoryUUID)
        {
            List<object> list = new List<object>(sourceUUIDList.Length);
            foreach (string uuid in sourceUUIDList)
            {
                var a = new { identity = uuid };
                list.Add(a);
            }
            var data = new
            {
                source = list,
                destination = new { identity = targetDirectoryUUID }
            };
            return await PostAsync<GenericResult<int?>>(JsonConvert.SerializeObject(data), "/v2/files/copy", false);
        }

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
            return await PostAsync<GenericResult<int?>>(JsonConvert.SerializeObject(data), "/v2/files/delete", false);
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

            return await PostAsync<GenericResult<int?>>(JsonConvert.SerializeObject(data), "/v2/files/delete", false);
        }

        /// <summary>
        /// 重命名文件夹或文件
        /// </summary>
        /// <param name="SourceMeta">被重命名的项目</param>
        /// <param name="newName">新名称</param>
        /// <returns></returns>
        public async Task<GenericResult<bool>> Rename(string identity, string name)
        {
            var data = new { identity, name };
            return await PostAsync<GenericResult<bool>>(JsonConvert.SerializeObject(data), "/v2/files/rename", false);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="name">FileMeta的文件名</param>
        /// <param name="parentUUID">上传到哪个文件夹</param>
        /// <param name="hash">哈希</param>
        /// <param name="originalFilename">保存的源信息的文件名</param>
        /// <returns></returns>
        public async Task<GenericResult<UploadToken>> UploadFile(string name, string parentUUID = null, string parentPath = null, string hash = null, string originalFilename = null)
        {
            dynamic data = new ExpandoObject();
            data.name = name;

            if (parentUUID != null)
            {
                data.identity = parentUUID;
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

            return await PostAsync<GenericResult<UploadToken>>(JsonConvert.SerializeObject(data), "/v2/upload/token", false);
        }

        /// <summary>
        /// 该方法用于获取下载链接
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public async Task<GenericResult<FileMetaData>> GetDetailsByUUID(string identity)
        {
            var data = new { identity };
            return await PostAsync<GenericResult<FileMetaData>>(JsonConvert.SerializeObject(data), "/v2/files/get", false);
        }

        public async Task<GenericResult<RecoveryBoxPage>> GetRecoveryBoxPage(int page = 1, int pageSize = 20)
        {
            var data = new { page, pageSize };
            return await PostAsync<GenericResult<RecoveryBoxPage>>(JsonConvert.SerializeObject(data), "/v2/trash/page", false);
        }

        public async Task<GenericResult<PreviewVideoInformation>> VideoPreview(string identity)
        {
            var data = new { identity };
            return await PostAsync<GenericResult<PreviewVideoInformation>>(JsonConvert.SerializeObject(data), "/v2/preview/video",false);
        }

        public async Task<GenericResult<PreviewImageInformation>> ImagePreview(string identity)
        {
            throw new NotImplementedException();
#warning 这里的代码还没有写完
            //while (string.IsNullOrWhiteSpace(Token))
            //{
            //    LoginView GetToken = new LoginView();
            //    GetToken.ShowDialog();
            //}
            //Dictionary<string, string> data = new Dictionary<string, string>
            //{
            //    { "uuid", UUID },
            //    { "token", Token }
            //};
            //GenericResult<PreviewImageInformation> x = Post<GenericResult<PreviewImageInformation>>(JsonConvert.SerializeObject(data), "v1/preview/image");
            //Token = x.Token;
            //return x;
        }
    }
}
