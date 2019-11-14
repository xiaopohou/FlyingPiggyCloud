using Newtonsoft.Json;
using SixCloud.Models;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;

namespace QingzhenyunApis.QingzhenyunMethods
{
    internal sealed class Share : SixCloudMethordBase
    {
        /// <summary>
        /// 通过路径创建分享
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="password">分享密码，默认为公开</param>
        /// <param name="expire">过期时间，默认为不限</param>
        /// <param name="copyCountLeft">可转存次数，默认为不限</param>
        /// <returns></returns>
        public GenericResult<ShareMetaData> CreateByPath(string path, string password = null, long expire = 0, long copyCountLeft = 0)
        {
            dynamic data = new ExpandoObject();
            data.path = path;
            if (!string.IsNullOrWhiteSpace(password))
            {
                data.password = password;
            }
            if (expire != 0)
            {
                data.expire = expire;
            }
            if (copyCountLeft != 0)
            {
                data.copyCountLeft = copyCountLeft;
            }
            var x = Post<GenericResult<ShareMetaData>>(JsonConvert.SerializeObject(data), "v2/share/create", new Dictionary<string, string>
            {
                { "Qingzhen-Token",Token }
            }, out WebHeaderCollection webHeaderCollection);
            Token = webHeaderCollection.Get("qingzhen-token");
            return x;
        }

        public GenericResult<ShareMetaData> Get(string identity)
        {
            var data = new { identity };
            var x = Post<GenericResult<ShareMetaData>>(JsonConvert.SerializeObject(data), "v2/share/get", new Dictionary<string, string>
            {
                { "Qingzhen-Token",Token }
            }, out WebHeaderCollection webHeaderCollection);
            Token = webHeaderCollection.Get("qingzhen-token");
            return x;
        }

        public GenericResult<bool> Save(string identity, string path, string password = null)
        {
            dynamic data = new ExpandoObject();
            data.identity = identity;
            data.filePath = path;
            if (!string.IsNullOrWhiteSpace(password))
            {
                data.password = password;
            }
            var x = Post<GenericResult<bool>>(JsonConvert.SerializeObject(data), "v2/share/get", new Dictionary<string, string>
            {
                { "Qingzhen-Token",Token }
            }, out WebHeaderCollection webHeaderCollection);
            Token = webHeaderCollection.Get("qingzhen-token");
            return x;
        }

        public GenericResult<bool> Cancel(string path)
        {
            var data = new { path };
            var x = Post<GenericResult<bool>>(JsonConvert.SerializeObject(data), "v2/share/cancel", new Dictionary<string, string>
            {
                { "Qingzhen-Token",Token }
            }, out WebHeaderCollection webHeaderCollection);
            Token = webHeaderCollection.Get("qingzhen-token");
            return x;
        }
    }
}