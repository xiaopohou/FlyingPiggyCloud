namespace Wangsu.WcsLib.Core
{
    internal class Config
    {
        public Config(string uploadHost = null, bool useHttp = false)
        {
            // set default
            UploadHost = uploadHost ?? "apitestuser.up0.v1.wcsapi.com";
            UseHttps = useHttp;
        }

        /// <summary>
        /// 获取资源管理域名
        /// </summary>
        /// <returns></returns>
        public string GetUploadUrlPrefix()
        {
            return UploadHost;
            //return (UseHttps ? "https://" : "http://") + UploadHost;
        }

        /// <summary>
        /// 上传域名
        /// </summary>
        public string UploadHost { set; get; }

        /// <summary>
        /// 是否采用 HTTPS 域名
        /// </summary>
        public bool UseHttps { set; get; }

        public const long BLOCK_SIZE = 4 * 1024 * 1024;
    }
}
