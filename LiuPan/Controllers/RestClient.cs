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

        #region Delete方式
        public static string Delete(string data, string uri)
        {
            return CommonHttpRequest(data, uri, "DELETE");
        }

        public static string Delete(string uri)
        {
            //Web访问对象64
            string serviceUrl = $"{BaseUri}/{uri}";
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            myRequest.Method = "DELETE";
            // 获得接口返回值68
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            //string ReturnXml = HttpUtility.UrlDecode(reader.ReadToEnd());
            string ReturnXml = reader.ReadToEnd();
            reader.Close();
            myResponse.Close();
            return ReturnXml;
        }
        #endregion

        #region Put方式
        public static string Put(string data, string uri)
        {
            return CommonHttpRequest(data, uri, "PUT");
        }
        #endregion

        #region POST方式
        public static string Post(string data, string uri)
        {
            return CommonHttpRequest(data, uri, "POST");
        }
        #endregion

        #region GET方式
        public static string Get(string uri)
        {
            //Web访问对象64
            string serviceUrl = string.Format("{0}/{1}", BaseUri, uri);

            //构造一个Web请求的对象
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            // 获得接口返回值68
            //获取web请求的响应的内容
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();

            //通过响应流构造一个StreamReader
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            //string ReturnXml = HttpUtility.UrlDecode(reader.ReadToEnd());
            string ReturnXml = reader.ReadToEnd();
            reader.Close();
            myResponse.Close();
            return ReturnXml;
        }
        #endregion

        public static string CommonHttpRequest(string data, string uri, string type)
        {
            //Web访问对象，构造请求的url地址
            string serviceUrl = string.Format("{0}/{1}", BaseUri, uri);
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
            // 发送请求
            Stream newStream = myRequest.GetRequestStream();
            newStream.Write(buf, 0, buf.Length);
            newStream.Close();
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
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string ReturnXml = reader.ReadToEnd();
            reader.Close();
            myResponse.Close();
            return ReturnXml;
        }
    }
}
