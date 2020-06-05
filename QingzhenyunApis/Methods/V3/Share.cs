using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System.Dynamic;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
    public sealed class Share : SixCloudMethodBase
    {
        /// <summary>
        /// 通过路径创建分享
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="password">分享密码，默认为公开</param>
        /// <param name="expire">过期时间，默认为不限</param>
        /// <param name="copyCountLeft">可转存次数，默认为不限</param>
        /// <returns></returns>
        public async Task<ShareMetaData> CreateByPath(string path, string password = null, long expire = 0, long copyCountLeft = 0)
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
            return await PostAsync<ShareMetaData>(JsonConvert.SerializeObject(data), "/v2/share/create");
        }

        public async Task<ShareMetaData> Get(string identity)
        {
            var data = new { identity };
            return await PostAsync<ShareMetaData>(JsonConvert.SerializeObject(data), "/v2/share/get");
        }

        public async Task<bool> Save(string identity, string path, string password = null)
        {
            dynamic data = new ExpandoObject();
            data.identity = identity;
            data.filePath = path;
            if (!string.IsNullOrWhiteSpace(password))
            {
                data.password = password;
            }
            return await PostAsync<bool>(JsonConvert.SerializeObject(data), "/v2/share/get");
        }

        public async Task<bool> Cancel(string path)
        {
            var data = new { path };
            return await PostAsync<bool>(JsonConvert.SerializeObject(data), "/v2/share/cancel");
        }
    }
}