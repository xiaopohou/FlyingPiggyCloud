using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SixCloud.Controllers
{
    /// <summary>
    /// HTTP请求
    /// </summary>
    public static class RestClient
    {
        private const string BaseUri = "https://api.6pan.cn";
        //#region Delete方式
        //public static string Delete(string data, string uri)
        //{
        //    return CommonHttpRequest(data, uri, "DELETE");
        //}

        //public static string Delete(string uri)
        //{
        //    //Web访问对象64
        //    string serviceUrl = $"{BaseUri}/{uri}";
        //    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
        //    myRequest.Method = "DELETE";
        //    // 获得接口返回值68
        //    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
        //    StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
        //    //string ReturnXml = HttpUtility.UrlDecode(reader.ReadToEnd());
        //    string ReturnXml = reader.ReadToEnd();
        //    reader.Close();
        //    myResponse.Close();
        //    return ReturnXml;
        //}
        //#endregion

        //#region Put方式
        //public static string Put(string data, string uri)
        //{
        //    return CommonHttpRequest(data, uri, "PUT");
        //}
        //#endregion

        //#region POST方式
        //public static string Post(string data, string uri)
        //{
        //    return CommonHttpRequest(data, uri, "POST");
        //}
        //#endregion

        //#region GET方式
        //public static string Get(string uri)
        //{
        //    //Web访问对象64
        //    string serviceUrl = string.Format("{0}/{1}", BaseUri, uri);

        //    //构造一个Web请求的对象
        //    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
        //    // 获得接口返回值68
        //    //获取web请求的响应的内容
        //    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();

        //    //通过响应流构造一个StreamReader
        //    StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
        //    //string ReturnXml = HttpUtility.UrlDecode(reader.ReadToEnd());
        //    string ReturnXml = reader.ReadToEnd();
        //    reader.Close();
        //    myResponse.Close();
        //    return ReturnXml;
        //}
        //#endregion

        //public static string CommonHttpRequest(string data, string uri, string type)
        //{
        //    //Web访问对象，构造请求的url地址
        //    string serviceUrl = string.Format("{0}/{1}", BaseUri, uri);
        //    //构造http请求的对象
        //    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
        //    //转成网络流
        //    byte[] buf = Encoding.GetEncoding("UTF-8").GetBytes(data);
        //    //设置
        //    myRequest.Method = type;
        //    myRequest.ContentLength = buf.Length;
        //    myRequest.ContentType = "application/json";
        //    myRequest.MaximumAutomaticRedirections = 1;
        //    myRequest.AllowAutoRedirect = true;
        //    // 发送请求
        //    Stream newStream = myRequest.GetRequestStream();
        //    newStream.Write(buf, 0, buf.Length);
        //    newStream.Close();
        //    // 获得接口返回值
        //    HttpWebResponse myResponse;
        //    try
        //    {
        //        myResponse = (HttpWebResponse)myRequest.GetResponse();
        //    }
        //    catch (WebException e)
        //    {
        //        myResponse = (HttpWebResponse)e.Response;
        //    }
        //    StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
        //    string ReturnXml = reader.ReadToEnd();
        //    reader.Close();
        //    myResponse.Close();
        //    return ReturnXml;
        //}
        private const string AccessKeyId = "dingding";

        private const string AccessKeySecret = "张宝华";

        public static HttpWebResponse CommonHttpRequest(string data, string uri, string type, Dictionary<string, string> headers)
        {
            //Web访问对象，构造请求的url地址
            string serviceUrl = $"{BaseUri}/{uri}";
            //构造http请求的对象
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            //转成网络流
            byte[] buf = Encoding.GetEncoding("UTF-8").GetBytes(data);
            //设置
            myRequest.Method = type;
            myRequest.ContentLength = buf.Length;
            myRequest.ContentType = "application/json";
            myRequest.MaximumAutomaticRedirections = 1;
            myRequest.AllowAutoRedirect = true;
            //myRequest.Headers.Add()
            string unixDateTimeNow = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString();
            headers = headers ?? new Dictionary<string, string>();
            headers.Add("contentMD5", MD5Calculater(buf));
            string canonicalizedResource = $"/{uri}";
            string[] headerKeys = new string[headers.Count];
            headers.Keys.CopyTo(headerKeys, 0);
            Array.Sort(headerKeys);
            StringBuilder headerBuilder = new StringBuilder();
            foreach (string key in headerKeys)
            {
                headerBuilder.Append($"{key.ToLower()}: {headers[key]}");
                myRequest.Headers.Add(key, headers[key]);
            }
            string signature = HmacSha1(AccessKeySecret, $"{type}{unixDateTimeNow}{headerBuilder.ToString()}{canonicalizedResource}");
            string authorization = $"Qingzhen {AccessKeyId}:{signature}";
            // 发送请求
            using (Stream newStream = myRequest.GetRequestStream())
            {
                newStream.Write(buf, 0, buf.Length);

            }
            // 获得接口返回值
            HttpWebResponse myResponse;
            try
            {
                myResponse = (HttpWebResponse)myRequest.GetResponse();
            }
            catch (WebException e)
            {
                myResponse = (HttpWebResponse)e.Response;
            }
            //using (StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8))
            //{
            //    string ReturnXml = reader.ReadToEnd();
            //}
            //myResponse.Close();
            return myResponse;

            string MD5Calculater(byte[] input)
            {
                MD5 md5 = MD5.Create();
                byte[] bytes = md5.ComputeHash(input);
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("X2"));
                }
                return builder.ToString();
            }

            string HmacSha1(string key, string input)
            {
                byte[] keyBytes = Encoding.ASCII.GetBytes(key);
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                HMACSHA1 hmac = new HMACSHA1(keyBytes);
                byte[] hashBytes = hmac.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        #region POST方式
        public static string Post(string data, string uri, Dictionary<string, string> requestHeaders, out Dictionary<string, string> headers)
        {
            using (HttpWebResponse response = CommonHttpRequest(data, uri, "POST", requestHeaders))
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    if (response.Headers.Count > 0)
                    {
                        headers = new Dictionary<string, string>(response.Headers.Count);
                        foreach (string head in response.Headers.AllKeys)
                        {
                            headers.Add(head, response.Headers.Get(head));
                        }
                    }
                    else
                    {
                        headers = null;
                    }
                    return reader.ReadToEnd();
                }
            }
        }

        public static string Post(string data, string uri, Dictionary<string, string> requestHeaders, out WebHeaderCollection responseHealders)
        {
            using (HttpWebResponse response = CommonHttpRequest(data, uri, "POST", requestHeaders))
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseHealders = response.Headers;
                    return reader.ReadToEnd();
                }
            }
        }
        #endregion

    }
}
