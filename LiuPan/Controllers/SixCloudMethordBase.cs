using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace SixCloud.Controllers
{
    /// <summary>
    /// APP生命周期内，所有派生于此抽象类的对象，均拥有共同的Token字段
    /// </summary>
    internal abstract class SixCloudMethordBase
    {
        protected static string Token { get; set; }

        protected T Post<T>(string data, string uri, Dictionary<string, string> requestHeaders, out WebHeaderCollection responseHeaders)
        {
            string p = RestClient.Post(data, uri, requestHeaders, out responseHeaders);
            return JsonConvert.DeserializeObject<T>(p);
        }

        //protected T Get<T>(string uri)
        //{
        //    return JsonConvert.DeserializeObject<T>(RestClient.Get(uri));
        //}

        public SixCloudMethordBase()
        {
            if (Models.LocalProperties.IsAutoLogin && Token == null)
            {
                Token = Models.LocalProperties.Token;
            }
        }

        ~SixCloudMethordBase()
        {
            if (Models.LocalProperties.IsAutoLogin && Token != null)
            {
                Models.LocalProperties.Token = Token;
            }
        }
    }
}