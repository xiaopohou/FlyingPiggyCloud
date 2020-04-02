using Newtonsoft.Json;
using QingzhenyunApis.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QingzhenyunApis.Methods.V3
{
    /// <summary>
    /// APP生命周期内，所有派生于此抽象类的对象，均拥有共同的Token字段
    /// </summary>
    public abstract class SixCloudMethodBase
    {

        private const string AccessKeyId = "bc088aa5e2ad";
        private const string AccessKeySecret = "DyO04JriYoqJ9f57";
        private static readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("https://api.6pan.cn") };

        private static string HmacSha1(string key, string input)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            using (HMACSHA1 hmac = new HMACSHA1(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

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
        private static void CreateSignature(ref string uri, bool isAnonymous, Dictionary<string, string> querys, HttpContentHeaders headers = null)
        {
            lock (Token)
            {

                string extraHeaders = $"{(isAnonymous ? "" : $"authorization: Bearer {Token}")}{(headers == null ? "" : $"content-md5: {BitConverter.ToString(headers.ContentMD5).Replace("-", "")}")}";

                if (querys != null)
                {
                    querys = new Dictionary<string, string>(3);
                }
                string unixDateTimeNow = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
                querys["appid"] = AccessKeyId;
                querys["ts"] = unixDateTimeNow;
                querys["nonce"] = Guid.NewGuid().ToString();

                IOrderedEnumerable<KeyValuePair<string, string>> queryStrings = from kvPair in querys
                                                                                orderby kvPair.Key
                                                                                select kvPair;
                StringBuilder uriBuilder = new StringBuilder(uri);
                uriBuilder.Append("?");
                foreach (KeyValuePair<string, string> query in queryStrings)
                {
                    uriBuilder.Append($"{query.Key}={HttpUtility.UrlEncode(query.Value)}&");
                }
                //移除最后一个&
                uriBuilder.Remove(uriBuilder.Length - 1, 1);

                string signature = HmacSha1(AccessKeySecret, $"POSTapi.6pan.cn{uriBuilder.ToString()}{extraHeaders}");
                uriBuilder.Append($"&signature={Calculators.Base64.Base64Encode(signature)}");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                uri = uriBuilder.ToString();
            }
        }

        protected static string Token { get; private set; } = string.Empty;

        protected static async Task<T> PostAsync<T>(string data, string uri, bool isAnonymous = false)
        {
            using (StringContent requestObject = new StringContent(data))
            {
                //构建请求头
                HttpContentHeaders headers = CreateHeader(data, requestObject);

                //构建签名
                CreateSignature(ref uri, isAnonymous, null, headers);

                //发起请求
                HttpResponseMessage response = await httpClient.PostAsync(uri, requestObject);

                if (response.Headers.TryGetValues("qingzhen-token", out IEnumerable<string> newToken))
                {
                    Token = newToken.FirstOrDefault() ?? Token;
                }
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(responseBody);
            }
        }

        protected static T Post<T>(string data, string uri, bool isAnonymous = true)
        {
            using (StringContent requestObject = new StringContent(data))
            {
                //构建请求头
                HttpContentHeaders headers = CreateHeader(data, requestObject);

                //构建签名
                CreateSignature(ref uri, isAnonymous, null, headers);

                //发起请求
                HttpResponseMessage response = httpClient.PostAsync(uri, requestObject).Result;
                string responseBody = response.Content.ReadAsStringAsync().Result;
                if (response.Headers.TryGetValues("qingzhen-token", out IEnumerable<string> newToken))
                {
                    Token = newToken.FirstOrDefault() ?? Token;
                }
                return JsonConvert.DeserializeObject<T>(responseBody);
            }
        }

        protected static async Task<T> GetAsync<T>(string uri, Dictionary<string, string> querys = null, bool isAnonymous = false)
        {
            //发起请求
            HttpResponseMessage response = await httpClient.GetAsync(uri);

            if (response.Headers.TryGetValues("qingzhen-token", out IEnumerable<string> newToken))
            {
                Token = newToken.FirstOrDefault() ?? Token;
            }
            string responseBody = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        protected SixCloudMethodBase(string token = null)
        {
            Token = token ?? Token;
        }

    }
}