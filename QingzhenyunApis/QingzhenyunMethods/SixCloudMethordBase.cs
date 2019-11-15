using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QingzhenyunApis.QingzhenyunMethods
{
    /// <summary>
    /// APP生命周期内，所有派生于此抽象类的对象，均拥有共同的Token字段
    /// </summary>
    internal abstract class SixCloudMethordBase
    {
        private const string AccessKeyId = "dingding";

        private const string AccessKeySecret = "张宝华";

        private static readonly HttpClient httpClient;

        protected static string Token { get; private set; }

        static SixCloudMethordBase()
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.6pan.cn")
            };
        }

        private string HmacSha1(string key, string input)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            using (HMACSHA1 hmac = new HMACSHA1(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        protected SixCloudMethordBase(string token = null)
        {
            Token = token ?? Token;
        }

        protected async Task<T> PostAsync<T>(string data, string uri, bool isAnonymous = true)
        {
            using (StringContent requestObject = new StringContent(data))
            {
                //构建请求头
                HttpContentHeaders headers = requestObject.Headers;
                headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                using (MD5 md5 = MD5.Create())
                {
                    headers.ContentMD5 = md5.ComputeHash(Encoding.GetEncoding("UTF-8").GetBytes(data));
                }

                //构建签名
                string unixDateTimeNow = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString();
                string extraHeaders = $"contentmd5: {BitConverter.ToString(headers.ContentMD5).Replace("-", "")}{(isAnonymous ? "" : $"qingzhen-token: {Token}")}";
                string signature = HmacSha1(AccessKeySecret, $"POST{unixDateTimeNow}{extraHeaders}{uri}");
                string authorization = $"Qingzhen {AccessKeyId}:{signature}";
                headers.Add(nameof(authorization), authorization);

                //发起请求
                HttpResponseMessage response = await httpClient.PostAsync(uri, requestObject);
                string responseBody = await response.Content.ReadAsStringAsync();
                Token = response.Headers.GetValues("qingzhen-token").FirstOrDefault() ?? Token;
                return JsonConvert.DeserializeObject<T>(responseBody);
            }

        }
    }
}