using Newtonsoft.Json;
using QingzhenyunApis.Utils;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
    /// <summary>
    /// APP生命周期内，所有派生于此抽象类的对象，均拥有共同的Token字段
    /// </summary>
    public abstract class SixCloudMethodBase
    {
        private const string AccessKeyId = "bc088aa5e2ad";

        private const string AccessKeySecret = "DyO04JriYoqJ9f57";

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

        private static readonly HttpClient httpClient;

        private static HttpContentHeaders CreateHeader(string data, StringContent requestObject)
        {
            HttpContentHeaders headers = requestObject.Headers;
            headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            using (MD5 md5 = MD5.Create())
            {
                headers.ContentMD5 = md5.ComputeHash(Encoding.GetEncoding("UTF-8").GetBytes(data));
            }

            return headers;
        }

        /// <summary>
        /// 构建签名
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="isAnonymous"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        private async Task CreateSignature(string uri, bool isAnonymous, HttpContentHeaders headers)
        {
            await Task.Run(() =>
            {
                lock (Token)
                {
                    string unixDateTimeNow = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString();
                    string extraHeaders = $"contentmd5: {BitConverter.ToString(headers.ContentMD5).Replace("-", "")}{(isAnonymous ? "" : $"qingzhen-token: {Token}")}";
                    string signature = HmacSha1(AccessKeySecret, $"POST{unixDateTimeNow}{extraHeaders}{uri}");
                    Calculators.Base64.Base64Encode(signature);

                    string authorization = $"{AccessKeyId}:{signature}";
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Qingzhen", authorization);

                    if (!isAnonymous)
                    {
                        headers.Add("qingzhen-token", Token);
                    }
                }
            });
        }

        static SixCloudMethodBase()
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.6pan.cn")
            };
        }

        protected static string Token { get; private set; } = string.Empty;

        protected SixCloudMethodBase(string token = null)
        {
            Token = token ?? Token;
        }

        protected async Task<T> PostAsync<T>(string data, string uri, bool isAnonymous = false)
        {
            using (StringContent requestObject = new StringContent(data))
            {
                //构建请求头
                HttpContentHeaders headers = CreateHeader(data, requestObject);

                //构建签名
                await CreateSignature(uri, isAnonymous, headers);

                //发起请求
                HttpResponseMessage response = await httpClient.PostAsync(uri, requestObject);
                if (response.Headers.TryGetValues("qingzhen-token", out var newToken))
                {
                    Token = newToken.FirstOrDefault() ?? Token;
                }
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(responseBody);
            }
        }

        protected T Post<T>(string data, string uri, bool isAnonymous = true)
        {
            using (StringContent requestObject = new StringContent(data))
            {
                //构建请求头
                HttpContentHeaders headers = CreateHeader(data, requestObject);

                //构建签名
                CreateSignature(uri, isAnonymous, headers).Wait();

                //发起请求
                HttpResponseMessage response = httpClient.PostAsync(uri, requestObject).Result;
                string responseBody = response.Content.ReadAsStringAsync().Result;
                if (response.Headers.TryGetValues("qingzhen-token", out var newToken))
                {
                    Token = newToken.FirstOrDefault() ?? Token;
                }
                return JsonConvert.DeserializeObject<T>(responseBody);
            }
        }


        protected async Task<T> GetAsync<T>(string uri, bool isAnonymous = false)
        {
            //发起请求
            HttpResponseMessage response = await httpClient.GetAsync(uri);

            if (response.Headers.TryGetValues("qingzhen-token", out var newToken))
            {
                Token = newToken.FirstOrDefault() ?? Token;
            }
            string responseBody = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseBody);
        }

    }
}