//using Newtonsoft.Json;
//using SixCloud.Models;
//using SixCloud.Views;
//using System;
//using System.Collections.Generic;
//using System.Net;

//namespace SixCloud.Controllers
//{
//    internal sealed class FileSystem : SixCloudMethordBase
//    {
//        /// <summary>
//        /// 请求的时候，如果 name 和 path均为空，则会返回根目录
//        /// </summary>
//        /// <param name="parent">该文件夹的ID</param>
//        /// <param name="path">路径</param>
//        /// <param name="page">第几页</param>
//        /// <param name="pageSize">列表大小，最大值999</param>
//        /// <param name="OrderBy">排序：0按文件名，1按时间</param>
//        /// <param name="Type">文件类型：0显示文件，1显示文件夹，-1显示文件和文件夹(默认)</param>
//        /// <returns></returns>
//        public GenericResult<FileListPage> GetDirectory(string parent = "", string path = "", int? page = null, int? pageSize = null, int orderBy = 1)
//        {
//            Dictionary<string, object> data = new Dictionary<string, object>();
//            if (parent != "")
//            {
//                data.Add("identity", parent);
//            }
//            else if (path != "")
//            {
//                data.Add("path", path);
//            }
//            if (page != null)
//            {
//                data.Add("page", page);
//            }
//            if (pageSize != null)
//            {
//                data.Add("pageSize", pageSize);
//            }
//            data.Add("orderBy", 1);
//            while (string.IsNullOrWhiteSpace(Token))
//            {
//                LoginView GetToken = new LoginView();
//                GetToken.ShowDialog();
//            }
//            data.Add("token", Token);
//            GenericResult<FileListPage> x = Post<GenericResult<FileListPage>>(JsonConvert.SerializeObject(data), "v2/files/page", new Dictionary<string, string>
//            {
//                { "Qingzhen-Token",Token }
//            }, out WebHeaderCollection webHeaderCollection);
//            Token = webHeaderCollection.Get("qingzhen-token");
//            return x;
//        }

//        /// <summary>
//        /// 新建文件夹，Parent和Path参数不可共存
//        /// </summary>
//        /// <param name="Name">新文件夹的名字，如果其他参数为空则在根目录创建此文件夹</param>
//        /// <param name="Parent">在UUID为此的文件夹内创建新文件夹</param>
//        /// <param name="Path">在此路径下创建新文件夹，如果Name参数为空则创建此路径</param>
//        /// <returns></returns>
//        public GenericResult<FileMetaData> CreatDirectory(string Name = "", string Parent = "", string Path = "")
//        {
//            Dictionary<string, string> data = new Dictionary<string, string>();
//            if (Name != "")
//            {
//                data.Add("name", Name);
//            }
//            if (Parent != "")
//            {
//                data.Add("parent", Parent);
//            }
//            else if (Path != "")
//            {
//                data.Add("path", Path);
//            }
//            while (string.IsNullOrWhiteSpace(Token))
//            {
//                LoginView GetToken = new LoginView();
//                GetToken.ShowDialog();
//            }
//            data.Add("token", Token);
//            GenericResult<FileMetaData> x = Post<GenericResult<FileMetaData>>(JsonConvert.SerializeObject(data), "v2/files/createDirectory", new Dictionary<string, string>
//            {
//                { "Qingzhen-Token",Token }
//            }, out WebHeaderCollection webHeaderCollection);
//            Token = webHeaderCollection.Get("qingzhen-token");
//            return x;
//        }

//        /// <summary>
//        /// 移动文件夹或文件
//        /// </summary>
//        /// <param name="SourceMeta">被移动的项目</param>
//        /// <param name="TargetDirectory">目标位置，必须是一个文件夹的Meta信息</param>
//        /// <returns></returns>
//        public GenericResult<int?> Move(string sourceUUID, string targetDirectoryUUID)
//        {
//            return Move(new string[] { sourceUUID }, targetDirectoryUUID);
//        }

//        /// <summary>
//        /// 移动一组文件或文件夹到同一个目录
//        /// </summary>
//        /// <param name="sourceUUIDList"></param>
//        /// <param name="targetDirectoryUUID"></param>
//        /// <returns></returns>
//        public GenericResult<int?> Move(string[] sourceUUIDList, string targetDirectoryUUID)
//        {
//            List<object> list = new List<object>(sourceUUIDList.Length);
//            foreach (string uuid in sourceUUIDList)
//            {
//                var a = new { identity = uuid };
//                list.Add(a);
//            }
//            Dictionary<string, object> data = new Dictionary<string, object>
//            {
//                { "source",list},
//                { "destination", new { identity=targetDirectoryUUID } }
//            };
//            while (string.IsNullOrWhiteSpace(Token))
//            {
//                LoginView GetToken = new LoginView();
//                GetToken.ShowDialog();
//            }
//            GenericResult<int?> x = Post<GenericResult<int?>>(JsonConvert.SerializeObject(data), "v2/files/move", new Dictionary<string, string>
//            {
//                { "Qingzhen-Token",Token }
//            }, out WebHeaderCollection webHeaderCollection);
//            Token = webHeaderCollection.Get("qingzhen-token");
//            return x;
//        }

//        /// <summary>
//        /// 复制文件夹或文件
//        /// </summary>
//        /// <param name="SourceMeta">被复制的项目</param>
//        /// <param name="TargetDirectory">目标位置，必须是一个文件夹的Meta信息</param>
//        /// <returns></returns>
//        public GenericResult<int?> Copy(string sourceUUID, string targetDirectoryUUID)
//        {
//            return Copy(new string[] { sourceUUID }, targetDirectoryUUID);
//        }

//        /// <summary>
//        /// 复制一组文件夹或文件
//        /// </summary>
//        /// <param name="sourceUUIDList"></param>
//        /// <param name="targetDirectoryUUID"></param>
//        /// <returns></returns>
//        public GenericResult<int?> Copy(string[] sourceUUIDList, string targetDirectoryUUID)
//        {
//            List<object> list = new List<object>(sourceUUIDList.Length);
//            foreach (string uuid in sourceUUIDList)
//            {
//                var a = new { identity = uuid };
//                list.Add(a);
//            }
//            Dictionary<string, object> data = new Dictionary<string, object>
//            {
//                { "source",list},
//                { "destination", new { identity=targetDirectoryUUID } }
//            };
//            while (string.IsNullOrWhiteSpace(Token))
//            {
//                LoginView GetToken = new LoginView();
//                GetToken.ShowDialog();
//            }
//            GenericResult<int?> x = Post<GenericResult<int?>>(JsonConvert.SerializeObject(data), "v2/files/copy", new Dictionary<string, string>
//            {
//                { "Qingzhen-Token",Token }
//            }, out WebHeaderCollection webHeaderCollection);
//            Token = webHeaderCollection.Get("qingzhen-token");
//            return x;
//        }

//        /// <summary>
//        /// 删除文件夹或文件
//        /// </summary>
//        /// <param name="uuid">被删除的项目</param>
//        /// <returns></returns>
//        public GenericResult<int?> Remove(string uuid)
//        {
//            Dictionary<string, object> data = new Dictionary<string, object>
//            {
//                { "source",new object[]{ new Dictionary<string, string> { { "identity", uuid } } } },
//            };
//            while (string.IsNullOrWhiteSpace(Token))
//            {
//                LoginView GetToken = new LoginView();
//                GetToken.ShowDialog();
//            }
//            GenericResult<int?> x = Post<GenericResult<int?>>(JsonConvert.SerializeObject(data), "v2/files/delete", new Dictionary<string, string>
//            {
//                { "Qingzhen-Token",Token }
//            }, out WebHeaderCollection webHeaderCollection);
//            Token = webHeaderCollection.Get("qingzhen-token");
//            return x;
//        }

//        /// <summary>
//        /// 删除文件夹或文件
//        /// </summary>
//        /// <param name="uuid">被删除的项目</param>
//        /// <returns></returns>
//        public GenericResult<int?> Remove(string[] uuids)
//        {
//            List<object> list = new List<object>(uuids.Length);
//            foreach (string uuid in uuids)
//            {
//                list.Add(new { identity = uuid });
//            }
//            Dictionary<string, object> data = new Dictionary<string, object>
//            {
//                { "source",list.ToArray() },
//            };
//            while (string.IsNullOrWhiteSpace(Token))
//            {
//                LoginView GetToken = new LoginView();
//                GetToken.ShowDialog();
//            }
//            GenericResult<int?> x = Post<GenericResult<int?>>(JsonConvert.SerializeObject(data), "v2/files/delete", new Dictionary<string, string>
//            {
//                { "Qingzhen-Token",Token }
//            }, out WebHeaderCollection webHeaderCollection);
//            Token = webHeaderCollection.Get("qingzhen-token");
//            return x;
//        }

//        /// <summary>
//        /// 重命名文件夹或文件
//        /// </summary>
//        /// <param name="SourceMeta">被重命名的项目</param>
//        /// <param name="newName">新名称</param>
//        /// <returns></returns>
//        public GenericResult<bool> Rename(string uuid, string newName)
//        {
//            while (string.IsNullOrWhiteSpace(Token))
//            {
//                LoginView GetToken = new LoginView();
//                GetToken.ShowDialog();
//            }
//            Dictionary<string, object> data = new Dictionary<string, object>
//            {
//                { "identity", uuid },
//                { "name", newName }
//            };
//            GenericResult<bool> x = Post<GenericResult<bool>>(JsonConvert.SerializeObject(data), "v2/files/rename", new Dictionary<string, string>
//            {
//                { "Qingzhen-Token",Token }
//            }, out WebHeaderCollection webHeaderCollection);
//            Token = webHeaderCollection.Get("qingzhen-token");
//            return x;
//        }

//        /// <summary>
//        /// 上传文件
//        /// </summary>
//        /// <param name="Name">FileMeta的文件名</param>
//        /// <param name="parentUUID">上传到哪个文件夹</param>
//        /// <param name="Hash">哈希</param>
//        /// <param name="OriginalFilename">保存的源信息的文件名</param>
//        /// <returns></returns>
//        public GenericResult<UploadToken> UploadFile(string Name, string parentUUID = null, string parentPath = null, string Hash = null, string OriginalFilename = null)
//        {
//            while (string.IsNullOrWhiteSpace(Token))
//            {
//                LoginView GetToken = new LoginView();
//                GetToken.ShowDialog();
//            }
//            Dictionary<string, string> data = new Dictionary<string, string>
//            {
//                { "name", Name},
//            };
//            if (parentUUID != null)
//            {
//                data.Add("identity", parentUUID);
//            }
//            else if (parentPath != null)
//            {
//                data.Add("path", parentPath);
//            }
//            else
//            {
//                data.Add("path", "/");
//            }

//            if (Hash != null)
//            {
//                data.Add("hash", Hash);
//            }

//            GenericResult<UploadToken> x = Post<GenericResult<UploadToken>>(JsonConvert.SerializeObject(data), "v2/upload/token", new Dictionary<string, string>
//            {
//                { "Qingzhen-Token",Token }
//            }, out WebHeaderCollection webHeaderCollection);
//            Token = webHeaderCollection.Get("qingzhen-token");
//            return x;
//        }

//        /// <summary>
//        /// 该方法用于获取下载链接
//        /// </summary>
//        /// <param name="UUID"></param>
//        /// <returns></returns>
//        public GenericResult<FileMetaData> GetDetailsByUUID(string UUID)
//        {
//            while (string.IsNullOrWhiteSpace(Token))
//            {
//                LoginView GetToken = new LoginView();
//                GetToken.ShowDialog();
//            }
//            Dictionary<string, string> data = new Dictionary<string, string>
//            {
//                { "identity", UUID },
//            };
//            GenericResult<FileMetaData> x = Post<GenericResult<FileMetaData>>(JsonConvert.SerializeObject(data), "v2/files/get", new Dictionary<string, string>
//            {
//                { "Qingzhen-Token",Token }
//            }, out WebHeaderCollection webHeaderCollection);
//            Token = webHeaderCollection.Get("qingzhen-token");
//            return x;

//        }

//        public GenericResult<RecoveryBoxPage> GetRecoveryBoxPage(int page = 1, int pageSize = 20)
//        {
//            while (string.IsNullOrWhiteSpace(Token))
//            {
//                LoginView GetToken = new LoginView();
//                GetToken.ShowDialog();
//            }
//            Dictionary<string, int> data = new Dictionary<string, int>
//            {
//                { "page", page },
//                { "pageSize", pageSize }
//            };
//            GenericResult<RecoveryBoxPage> x = Post<GenericResult<RecoveryBoxPage>>(JsonConvert.SerializeObject(data), "v2/trash/page", new Dictionary<string, string>
//            {
//                { "Qingzhen-Token",Token }
//            }, out WebHeaderCollection webHeaderCollection);
//            Token = webHeaderCollection.Get("qingzhen-token");
//            return x;

//        }

//        public GenericResult<PreviewVideoInformation> VideoPreview(string UUID)
//        {
//            while (string.IsNullOrWhiteSpace(Token))
//            {
//                LoginView GetToken = new LoginView();
//                GetToken.ShowDialog();
//            }
//            var data = new { identity = UUID };
//            GenericResult<PreviewVideoInformation> x = Post<GenericResult<PreviewVideoInformation>>(JsonConvert.SerializeObject(data), "v2/preview/video", new Dictionary<string, string>
//            {
//                { "Qingzhen-Token",Token }
//            }, out WebHeaderCollection webHeaderCollection);
//            Token = webHeaderCollection.Get("qingzhen-token");
//            return x;
//        }

//        public GenericResult<PreviewImageInformation> ImagePreview(string UUID)
//        {
//            throw new Exception();
//#warning 这里的代码还没有写完
//            //while (string.IsNullOrWhiteSpace(Token))
//            //{
//            //    LoginView GetToken = new LoginView();
//            //    GetToken.ShowDialog();
//            //}
//            //Dictionary<string, string> data = new Dictionary<string, string>
//            //{
//            //    { "uuid", UUID },
//            //    { "token", Token }
//            //};
//            //GenericResult<PreviewImageInformation> x = Post<GenericResult<PreviewImageInformation>>(JsonConvert.SerializeObject(data), "v1/preview/image");
//            //Token = x.Token;
//            //return x;
//        }
//    }
//}
