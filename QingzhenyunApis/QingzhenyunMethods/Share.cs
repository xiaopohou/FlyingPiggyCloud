using Newtonsoft.Json;
using QingzhenyunApis.QingzhenyunEntityModels;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Threading.Tasks;

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
        public async Task<GenericResult<ShareMetaData>> CreateByPath(string path, string password = null, long expire = 0, long copyCountLeft = 0)
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
            return await PostAsync<GenericResult<ShareMetaData>>(JsonConvert.SerializeObject(data), "/v2/share/create", false);
        }

        public async Task<GenericResult<ShareMetaData>> Get(string identity)
        {
            var data = new { identity };
            return await PostAsync<GenericResult<ShareMetaData>>(JsonConvert.SerializeObject(data), "/v2/share/get", false);
        }

        public async Task<GenericResult<bool>> Save(string identity, string path, string password = null)
        {
            dynamic data = new ExpandoObject();
            data.identity = identity;
            data.filePath = path;
            if (!string.IsNullOrWhiteSpace(password))
            {
                data.password = password;
            }
            return await PostAsync<GenericResult<bool>>(JsonConvert.SerializeObject(data), "/v2/share/get", false);
        }

        public async Task<GenericResult<bool>> Cancel(string path)
        {
            var data = new { path };
            return await PostAsync<GenericResult<bool>>(JsonConvert.SerializeObject(data), "/v2/share/cancel", false);
        }
    }
}