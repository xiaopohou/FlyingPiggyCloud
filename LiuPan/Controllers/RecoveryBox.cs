//using Newtonsoft.Json;
//using SixCloud.Models;
//using SixCloud.Views;
//using System.Collections.Generic;
//using System.Net;

//namespace SixCloud.Controllers
//{
//    internal sealed class RecoveryBox : SixCloudMethordBase
//    {
//        /// <summary>
//        /// 清空当前用户的回收站
//        /// </summary>
//        /// <returns></returns>
//        public GenericResult<int?> Empty()
//        {
//            while (string.IsNullOrWhiteSpace(Token))
//            {
//                LoginView GetToken = new LoginView();
//                GetToken.ShowDialog();
//            }
//            GenericResult<int?> x = Post<GenericResult<int?>>(JsonConvert.SerializeObject(new object()), "v2/trash/truncate", new Dictionary<string, string>
//            {
//                { "Qingzhen-Token",Token }
//            }, out WebHeaderCollection webHeaderCollection);
//            Token = webHeaderCollection.Get("qingzhen-token");
//            return x;
//        }

//        /// <summary>
//        /// 从回收站恢复文件
//        /// </summary>
//        /// <param name="targetUUID">待恢复的一组文件或文件夹</param>
//        /// <param name="parentUUID">恢复到其他文件夹</param>
//        /// <param name="parentPath">恢复到其他路径</param>
//        /// <returns></returns>
//        public GenericResult<int?> Restore(string[] targetUUID, string parentUUID = null, string parentPath = null)
//        {
//            Dictionary<string, object> data = new Dictionary<string, object>
//            {
//                { "source", targetUUID },
//            };
//            if (parentUUID != null)
//            {
//                data.Add("identity", parentUUID);
//            }
//            else if (parentPath != null)
//            {
//                data.Add("path", parentPath);
//            }
//            while (string.IsNullOrWhiteSpace(Token))
//            {
//                LoginView GetToken = new LoginView();
//                GetToken.ShowDialog();
//            }
//            GenericResult<int?> x = Post<GenericResult<int?>>(JsonConvert.SerializeObject(data), "v2/trash/moveFromTrash", new Dictionary<string, string>
//            {
//                { "Qingzhen-Token",Token }
//            }, out WebHeaderCollection webHeaderCollection);
//            Token = webHeaderCollection.Get("qingzhen-token");
//            return x;
//        }

//        /// <summary>
//        /// 从回收站删除文件
//        /// </summary>
//        /// <param name="targetUUID"></param>
//        /// <returns></returns>
//        public GenericResult<int?> Delete(string[] targetUUID)
//        {
//            //Dictionary<string, object> data = new Dictionary<string, object>
//            //{
//            //    { "identities", targetUUID },
//            //};
//            while (string.IsNullOrWhiteSpace(Token))
//            {
//                LoginView GetToken = new LoginView();
//                GetToken.ShowDialog();
//            }
//            GenericResult<int?> x = Post<GenericResult<int?>>(JsonConvert.SerializeObject(targetUUID), "v2/trash/delete", new Dictionary<string, string>
//            {
//                { "Qingzhen-Token",Token }
//            }, out WebHeaderCollection webHeaderCollection);
//            Token = webHeaderCollection.Get("qingzhen-token");
//            return x;
//        }

//        public GenericResult<RecoveryBoxPage> GetList(int page = 1, int pageSize = 20)
//        {
//            Dictionary<string, object> data = new Dictionary<string, object>
//            {
//                { "page", page },
//                { "pageSize", pageSize }
//            };
//            while (string.IsNullOrWhiteSpace(Token))
//            {
//                LoginView GetToken = new LoginView();
//                GetToken.ShowDialog();
//            }
//            GenericResult<RecoveryBoxPage> x = Post<GenericResult<RecoveryBoxPage>>(JsonConvert.SerializeObject(data), "v2/trash/page", new Dictionary<string, string>
//            {
//                { "Qingzhen-Token",Token }
//            }, out WebHeaderCollection webHeaderCollection);
//            Token = webHeaderCollection.Get("qingzhen-token");
//            return x;

//        }
//    }
//}
