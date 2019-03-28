﻿using Newtonsoft.Json;
using SixCloud.Models;
using SixCloud.Views;
using System.Collections.Generic;

namespace SixCloud.Controllers
{
    internal class FileSystem : SixCloudMethordBase
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
        public GenericResult<FileListPage> GetDirectory(string parent = "", string path = "", int? page = null, int? pageSize = null, int? OrderBy = null, int? Type = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (parent != "")
            {
                data.Add("parent", parent);
            }
            if (path != "")
            {
                data.Add("path", path);
            }
            if (page != null)
            {
                data.Add("page", page);
            }
            if (pageSize != null)
            {
                data.Add("pageSize", pageSize);
            }
            if (OrderBy != null)
            {
                data.Add("orderBy", OrderBy);
            }
            if (Type != null)
            {
                data.Add("type", Type);
            }
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            data.Add("token", Token);
            GenericResult<FileListPage> x = Post<GenericResult<FileListPage>>(JsonConvert.SerializeObject(data), "v1/files/page");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 新建文件夹，Parent和Path参数不可共存
        /// </summary>
        /// <param name="Name">新文件夹的名字，如果其他参数为空则在根目录创建此文件夹</param>
        /// <param name="Parent">在UUID为此的文件夹内创建新文件夹</param>
        /// <param name="Path">在此路径下创建新文件夹，如果Name参数为空则创建此路径</param>
        /// <returns></returns>
        public GenericResult<FileMetaData> CreatDirectory(string Name = "", string Parent = "", string Path = "")
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            if (Name != "")
            {
                data.Add("name", Name);
            }
            if (Parent != "")
            {
                data.Add("parent", Parent);
            }
            else if (Path != "")
            {
                data.Add("path", Path);
            }
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            data.Add("token", Token);
            GenericResult<FileMetaData> x = Post<GenericResult<FileMetaData>>(JsonConvert.SerializeObject(data), "v1/files/createDirectory");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 移动文件夹或文件
        /// </summary>
        /// <param name="SourceMeta">被移动的项目</param>
        /// <param name="TargetDirectory">目标位置，必须是一个文件夹的Meta信息</param>
        /// <returns></returns>
        public GenericResult<bool> Move(string sourceUUID, string targetDirectoryUUID)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "uuid", sourceUUID },
                { "parent", targetDirectoryUUID }
            };
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            data.Add("token", Token);
            GenericResult<bool> x = Post<GenericResult<bool>>(JsonConvert.SerializeObject(data), "v1/files/move");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 复制文件夹或文件
        /// </summary>
        /// <param name="SourceMeta">被复制的项目</param>
        /// <param name="TargetDirectory">目标位置，必须是一个文件夹的Meta信息</param>
        /// <returns></returns>
        public GenericResult<bool> Copy(string sourceUUID, string targetDirectoryUUID)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "uuid", sourceUUID },
                { "parent", targetDirectoryUUID }
            };
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            data.Add("token", Token);
            GenericResult<bool> x = Post<GenericResult<bool>>(JsonConvert.SerializeObject(data), "v1/files/copy");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 删除文件夹或文件
        /// </summary>
        /// <param name="uuid">被删除的项目</param>
        /// <returns></returns>
        public GenericResult<bool> Remove(string uuid)
        {
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "uuid", uuid }
            };
            data.Add("token", Token);
            GenericResult<bool> x = Post<GenericResult<bool>>(JsonConvert.SerializeObject(data), "v1/files/remove");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 重命名文件夹或文件
        /// </summary>
        /// <param name="SourceMeta">被重命名的项目</param>
        /// <param name="newName">新名称</param>
        /// <returns></returns>
        public void Rename(string uuid, string newName)
        {
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "uuid", uuid },
                { "name", newName },
                { "token", Token }
            };
            Dictionary<string, string> x = Post<Dictionary<string, string>>(JsonConvert.SerializeObject(data), "v1/files/rename");
            Token = x["token"];
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="Name">FileMeta的文件名</param>
        /// <param name="parentUUID">上传到哪个文件夹</param>
        /// <param name="Hash">哈希</param>
        /// <param name="OriginalFilename">保存的源信息的文件名</param>
        /// <returns></returns>
        public GenericResult<UploadToken> UploadFile(string Name, string parentUUID = null, string parentPath = null, string Hash = null, string OriginalFilename = null)
        {
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "name", Name},
                { "token", Token }
            };
            if (parentUUID != null)
            {
                data.Add("parent", parentUUID);
            }
            else if (parentPath != null)
            {
                data.Add("path", parentPath);
            }
            else
            {
                data.Add("path", "/");
            }

            if (Hash != null)
            {
                data.Add("hash", Hash);
            }

            if (OriginalFilename != null)
            {
                data.Add("originalFilename", OriginalFilename);
            }

            GenericResult<UploadToken> x = Post<GenericResult<UploadToken>>(JsonConvert.SerializeObject(data), "v1/store/token");
            Token = x.Token;
            return x;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UUID"></param>
        /// <returns></returns>
        public GenericResult<FileMetaData> GetDetailsByUUID(string UUID)
        {
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "uuid", UUID },
                { "token", Token }
            };
            GenericResult<FileMetaData> x = Post<GenericResult<FileMetaData>>(JsonConvert.SerializeObject(data), "v1/files/get");
            Token = x.Token;
            return x;

        }

        public GenericResult<PreviewVideoInformation> VideoPreview(string UUID)
        {
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "uuid", UUID },
                { "token", Token }
            };
            GenericResult<PreviewVideoInformation> x = Post<GenericResult<PreviewVideoInformation>>(JsonConvert.SerializeObject(data), "v1/preview/media");
            Token = x.Token;
            return x;
        }

        public GenericResult<PreviewImageInformation> ImagePreview(string UUID)
        {
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "uuid", UUID },
                { "token", Token }
            };
            GenericResult<PreviewImageInformation> x = Post<GenericResult<PreviewImageInformation>>(JsonConvert.SerializeObject(data), "v1/preview/image");
            Token = x.Token;
            return x;
        }
    }
}