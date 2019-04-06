//----------------------------------------------------------------------------------------------------
// <copyright company="Avira Operations GmbH & Co. KG and its licensors">
// ?2016 Avira Operations GmbH & Co. KG and its licensors.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace FileDownloader
{
    public class Cache : IDownloadCache
    {
        void IDownloadCache.Add(Uri uri, string path, WebHeaderCollection headers)
        {
            CachObject cach = new CachObject() { URI = uri.AbsoluteUri, PATH = path, HEADERS = headers.AllKeys };
            string jsonData = JsonConvert.SerializeObject(cach);
            string Path = Base64.Base64Encode(uri.AbsolutePath).Replace("\\", "");
            File.WriteAllText(System.IO.Path.GetTempPath() + Path, jsonData);
        }

        string IDownloadCache.Get(Uri uri, WebHeaderCollection headers)
        {
            string Path = Base64.Base64Encode(uri.AbsolutePath).Replace("\\", "");
            string PATH = System.IO.Path.GetTempPath() + Path;
            if (File.Exists(PATH))
            {
                string JsonEncoded = File.ReadAllText(PATH);
                CachObject account = JsonConvert.DeserializeObject<CachObject>(JsonEncoded);
                return account.PATH;
            }
            else
            {
                return null;
            }
        }

        void IDownloadCache.Invalidate(Uri uri)
        {
            string Path = Base64.Base64Encode(uri.AbsolutePath).Replace("\\", "");
            string PATH = System.IO.Path.GetTempPath() + Path;
            if (File.Exists(PATH))
            {
                File.Delete(PATH);
            }
        }
    }
}