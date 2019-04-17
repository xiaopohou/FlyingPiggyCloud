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
        }

        void IDownloadCache.Invalidate(Uri uri)
        {
            if (dictionary.ContainsKey(uri))
            {
                dictionary.Remove(uri);
            }
        }
    }
}