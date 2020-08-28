using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Utils;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
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
        protected const string AccessKeyId = "bc088aa5e2ad";
        private const string AccessKeySecret = "DyO04JriYoqJ9f57";

        private static readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("https://api.6pan.cn") };
        private static SocketIO socketIO;

        static SixCloudMethodBase()
        {
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"qingzhen uwp client {Assembly.GetEntryAssembly().GetName().Version}");
        }

        private static string HmacSha1(string key, string input)
        {
            var keyBytes = Encoding.ASCII.GetBytes(key);
            var inputBytes = Encoding.ASCII.GetBytes(input);
            using var hmac = new HMACSHA1(keyBytes);
            var hashBytes = hmac.ComputeHash(inputBytes);
            return Convert.ToBase64String(hashBytes);
        }

        private static HttpContentHeaders CreateHeader(string data, StringContent requestObject)
        {
            var headers = requestObject.Headers;
            headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            using (var md5 = MD5.Create())
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
        private static void CreateSignature(string method, ref string uri, bool isAnonymous, Dictionary<string, string> querys, HttpContentHeaders headers = null)
        {
            lock (Token)
            {

                var extraHeaders = $"{(isAnonymous ? "" : $"authorization: Bearer {Token}")}{(headers == null ? "" : $"content-md5: {BitConverter.ToString(headers.ContentMD5).Replace("-", "")}")}";

                if (querys == null)
                {
                    querys = new Dictionary<string, string>(3);
                }
                var unixDateTimeNow = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                querys["appid"] = AccessKeyId;
                querys["ts"] = unixDateTimeNow.ToString();
                querys["nonce"] = Guid.NewGuid().ToString();

                var queryStrings = from kvPair in querys
                                   orderby kvPair.Key
                                   select kvPair;
                var uriBuilder = new StringBuilder(uri);
                uriBuilder.Append("?");
                foreach (var query in queryStrings)
                {
                    uriBuilder.Append($"{query.Key}={HttpUtility.UrlEncode(query.Value)}&");
                }
                //移除最后一个&
                uriBuilder.Remove(uriBuilder.Length - 1, 1);

                var signature = HmacSha1(AccessKeySecret, $"{method}api.6pan.cn{uriBuilder}{extraHeaders}");
                uriBuilder.Append($"&signature={Calculators.Base64.Base64Encode(signature)}");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                uri = uriBuilder.ToString();
            }
        }

        private static T ParseResult<T>(string responseBody)
        {
            try
            {
                try
                {
                    var x = JsonConvert.DeserializeObject<T>(responseBody);
                    if (x is FileSystemOperate operate)
                    {
                        FileSystemOperateList[operate.Identity] = operate;
                    }
                    return x;
                }
                catch (JsonSerializationException)
                {
                    var ex = JsonConvert.DeserializeObject<RequestFailedException>(responseBody);
                    ex.Data[nameof(responseBody)] = responseBody;
                    throw ex;
                }
            }
            catch (JsonReaderException ex)
            {
                var exception = new RequestFailedException("Invalid response body as a json string", ex);
                exception.Data[nameof(responseBody)] = responseBody;
                throw exception;
            }
        }

        private static void SocketMessageHandler(string message)
        {
            try
            {
                var msg = ParseResult<SocketMessage>(message);
                FileSystemOperateList[msg.Identity]?.OnSocketMessageArrived(msg);
            }
            catch (RequestFailedException ex)
            {
                ex.Submit("UnkonwnSocketIOMessage");
            }
        }

        public static string Token { get; protected set; } = string.Empty;

        internal static Dictionary<string, FileSystemOperate> FileSystemOperateList { get; } = new Dictionary<string, FileSystemOperate>();

        protected static async void InitializeSocketClient(string uid)
        {
            if (socketIO == null)
            {
                return;
            }

            socketIO ??= new SocketIO("https://ws.6pan.cn/notice");

            socketIO.Options.Query["user"] = uid;

            socketIO.On("file.progress", (x) =>
            {
                SocketMessageHandler(x.GetValue<string>());
            });

            socketIO.On("file.complete", (x) =>
            {
                SocketMessageHandler(x.GetValue<string>());
            });

            socketIO.On("file.error", (x) =>
            {
                SocketMessageHandler(x.GetValue<string>());
            });

            socketIO.On("file.cancel", (x) =>
            {
                SocketMessageHandler(x.GetValue<string>());
            });

            await socketIO.ConnectAsync();
        }

        #region HTTP Methods

        protected static async Task<T> PostAsync<T>(string data, string uri, Dictionary<string, string> querys = null, bool isAnonymous = false)
        {
            using var requestObject = new StringContent(data);

            //构建请求头
            var headers = CreateHeader(data, requestObject);

            //构建签名
            CreateSignature("POST", ref uri, isAnonymous, querys, headers);

            try
            {
                //发起请求
                var response = isAnonymous ? await AnonymousPost(uri, requestObject) : await httpClient.PostAsync(uri, requestObject);

                if (response.Headers.TryGetValues("qingzhen-token", out var newToken))
                {
                    Token = newToken.FirstOrDefault() ?? Token;
                }
                var responseBody = await response.Content.ReadAsStringAsync();

                return ParseResult<T>(responseBody);
            }
            catch (Exception ex)
            {
                RequestFailedException exception;
                if (ex is RequestFailedException)
                {
                    exception = ex as RequestFailedException;
                }
                else
                {
                    exception = new RequestFailedException(ex.Message, ex);
                }

                exception.Data[nameof(data)] = data;
                exception.Data[nameof(uri)] = uri;
                exception.Data[nameof(querys)] = querys;
                exception.Data[nameof(isAnonymous)] = isAnonymous;

                throw exception;
            }


            static async Task<HttpResponseMessage> AnonymousPost(string uri, HttpContent httpContent)
            {
                using var httpClient = new HttpClient { BaseAddress = new Uri("https://api.6pan.cn") };
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"qingzhen uwp client {Assembly.GetEntryAssembly().GetName().Version}");
                return await httpClient.PostAsync(uri, httpContent);
            }
        }

        protected static async Task<T> GetAsync<T>(string uri, Dictionary<string, string> querys = null, bool isAnonymous = false)
        {
            //构建签名
            CreateSignature("GET", ref uri, isAnonymous, querys);

            try
            {
                //发起请求
                var response = isAnonymous ? await AnonymousGet(uri) : await httpClient.GetAsync(uri);
                if (response.Headers.TryGetValues("qingzhen-token", out var newToken))
                {
                    Token = newToken.FirstOrDefault() ?? Token;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                return ParseResult<T>(responseBody);

            }
            catch (Exception ex)
            {
                RequestFailedException exception;
                if (ex is RequestFailedException)
                {
                    exception = ex as RequestFailedException;
                }
                else
                {
                    exception = new RequestFailedException(ex.Message, ex);
                }
                exception.Data[nameof(uri)] = uri;
                exception.Data[nameof(querys)] = querys;
                exception.Data[nameof(isAnonymous)] = isAnonymous;
                throw exception;
            }


            static async Task<HttpResponseMessage> AnonymousGet(string uri)
            {
                using var httpClient = new HttpClient { BaseAddress = new Uri("https://api.6pan.cn") };
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"qingzhen uwp client {Assembly.GetEntryAssembly().GetName().Version}");
                return await httpClient.GetAsync(uri);
            }
        }

        protected static async Task<T> PatchAsync<T>(string data, string uri, Dictionary<string, string> querys = null, bool isAnonymous = false)
        {
            using var requestObject = new StringContent(data);
            //构建请求头
            var headers = CreateHeader(data, requestObject);

            //构建签名
            CreateSignature("PATCH", ref uri, isAnonymous, querys, headers);

            try
            {
                //发起请求
                var response = isAnonymous ? await AnonymousPatch(uri, requestObject) : await httpClient.PatchAsync(uri, requestObject);

                if (response.Headers.TryGetValues("qingzhen-token", out var newToken))
                {
                    Token = newToken.FirstOrDefault() ?? Token;
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                return ParseResult<T>(responseBody);
            }
            catch (Exception ex)
            {
                RequestFailedException exception;
                if (ex is RequestFailedException)
                {
                    exception = ex as RequestFailedException;
                }
                else
                {
                    exception = new RequestFailedException(ex.Message, ex);
                }

                exception.Data[nameof(data)] = data;
                exception.Data[nameof(uri)] = uri;
                exception.Data[nameof(querys)] = querys;
                exception.Data[nameof(isAnonymous)] = isAnonymous;
                throw exception;
            }


            static async Task<HttpResponseMessage> AnonymousPatch(string uri, HttpContent httpContent)
            {
                using var httpClient = new HttpClient { BaseAddress = new Uri("https://api.6pan.cn") };
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"qingzhen uwp client {Assembly.GetEntryAssembly().GetName().Version}");
                return await httpClient.PatchAsync(uri, httpContent);
            }
        }

        protected static async Task<T> PutAsync<T>(string data, string uri, Dictionary<string, string> querys = null, bool isAnonymous = false)
        {
            using var requestObject = new StringContent(data);
            //构建请求头
            var headers = CreateHeader(data, requestObject);

            //构建签名
            CreateSignature("PUT", ref uri, isAnonymous, querys, headers);

            try
            {
                //发起请求
                var response = isAnonymous ? await AnonymousPut(uri, requestObject) : await httpClient.PutAsync(uri, requestObject);

                if (response.Headers.TryGetValues("qingzhen-token", out var newToken))
                {
                    Token = newToken.FirstOrDefault() ?? Token;
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                return ParseResult<T>(responseBody);
            }
            catch (Exception ex)
            {
                RequestFailedException exception;
                if (ex is RequestFailedException)
                {
                    exception = ex as RequestFailedException;
                }
                else
                {
                    exception = new RequestFailedException(ex.Message, ex);
                }

                exception.Data[nameof(data)] = data;
                exception.Data[nameof(uri)] = uri;
                exception.Data[nameof(querys)] = querys;
                exception.Data[nameof(isAnonymous)] = isAnonymous;
                throw exception;
            }


            static async Task<HttpResponseMessage> AnonymousPut(string uri, HttpContent httpContent)
            {
                using var httpClient = new HttpClient { BaseAddress = new Uri("https://api.6pan.cn") };
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"qingzhen uwp client {Assembly.GetEntryAssembly().GetName().Version}");
                return await httpClient.PutAsync(uri, httpContent);
            }

        }

        protected static async Task<T> DeleteAsync<T>(string uri, Dictionary<string, string> querys = null, bool isAnonymous = false)
        {
            //构建签名
            CreateSignature("DELETE", ref uri, isAnonymous, querys);

            try
            {
                //发起请求
                var response = isAnonymous ? await AnonymousDelete(uri) : await httpClient.DeleteAsync(uri);

                if (response.Headers.TryGetValues("qingzhen-token", out var newToken))
                {
                    Token = newToken.FirstOrDefault() ?? Token;
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                return ParseResult<T>(responseBody);
            }
            catch (Exception ex)
            {
                RequestFailedException exception;
                if (ex is RequestFailedException)
                {
                    exception = ex as RequestFailedException;
                }
                else
                {
                    exception = new RequestFailedException(ex.Message, ex);
                }

                exception.Data[nameof(uri)] = uri;
                exception.Data[nameof(querys)] = querys;
                exception.Data[nameof(isAnonymous)] = isAnonymous;
                throw exception;
            }


            static async Task<HttpResponseMessage> AnonymousDelete(string uri)
            {
                using var httpClient = new HttpClient { BaseAddress = new Uri("https://api.6pan.cn") };
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"qingzhen uwp client {Assembly.GetEntryAssembly().GetName().Version}");
                return await httpClient.DeleteAsync(uri);
            }

        }
        #endregion

        protected SixCloudMethodBase(string token = null)
        {
            Token = token ?? Token;
        }
    }
}