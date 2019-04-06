//----------------------------------------------------------------------------------------------------
// <copyright company="Avira Operations GmbH & Co. KG and its licensors">
// ?2016 Avira Operations GmbH & Co. KG and its licensors.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;

namespace FileDownloader
{
    public class Cache : IDownloadCache
    {
        private static Dictionary<Uri, string> dictionary = new Dictionary<Uri, string>();

        void IDownloadCache.Add(Uri uri, string path, WebHeaderCollection headers)
        {
            lock (dictionary)
            {
                //dictionary.Add(uri, path);
                dictionary[uri] = path;
            }
            //CachObject cach = new CachObject() { URI = uri.AbsoluteUri, PATH = path, HEADERS = headers.AllKeys };
            //string jsonData = JsonConvert.SerializeObject(cach);
            //string Path = Base64.Base64Encode(uri.AbsolutePath).Replace("\\", "");
            //File.WriteAllText(System.IO.Path.GetTempPath() + Path, jsonData);
        }

        string IDownloadCache.Get(Uri uri, WebHeaderCollection headers)
        {
            try
            {
                lock (dictionary)
                {
                    return dictionary[uri];
                }
            }
            catch (Exception)
            {
                return null;
            }

            //string Path = Base64.Base64Encode(uri.AbsolutePath).Replace("\\", "");
            //string PATH = System.IO.Path.GetTempPath() + Path;
            //if (File.Exists(PATH))
            //{
            //    string JsonEncoded = File.ReadAllText(PATH);
            //    CachObject account = JsonConvert.DeserializeObject<CachObject>(JsonEncoded);
            //    return account.PATH;
            //}
            //else
            //{
            //    return null;
            //}
        }

        void IDownloadCache.Invalidate(Uri uri)
        {
            if (dictionary.ContainsKey(uri))
            {
                dictionary.Remove(uri);
            }
            //string Path = Base64.Base64Encode(uri.AbsolutePath).Replace("\\", "");
            //string PATH = System.IO.Path.GetTempPath() + Path;
            //if (File.Exists(PATH))
            //{
            //    File.Delete(PATH);
            //}
        }
    }
}