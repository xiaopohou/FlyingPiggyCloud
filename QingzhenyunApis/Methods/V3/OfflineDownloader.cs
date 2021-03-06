﻿using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
    public sealed class OfflineDownloader : SixCloudMethodBase
    {

        /// <summary>
        /// 预解析离线资源
        /// </summary>
        /// <param name="textLink">链接地址</param>
        /// <param name="fileHash">种子哈希</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="type">类型</param>
        /// <exception cref="UnsupportUrlException">不支持的链接</exception>
        /// <exception cref="NeedPasswordException">需要密码</exception>
        /// <returns></returns>
        public static async Task<OfflineTaskParseUrl> Parse(string textLink = null, string fileHash = null, string username = null, string password = null, int? type = null)
        {
            dynamic data = new ExpandoObject();
            if (!string.IsNullOrWhiteSpace(textLink))
            {
                data.textLink = textLink;
            }
            else if (!string.IsNullOrWhiteSpace(fileHash))
            {
                data.fileHash = fileHash;
            }
            else
            {
                throw new InvalidOperationException("Both textLink and fileHash are empty.");
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                data.username = username;
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                data.password = password;
            }

            if (type != null)
            {
                data.type = data;
            }
            try
            {
                return await PostAsync<OfflineTaskParseUrl>(JsonConvert.SerializeObject(data), "/v3/offline/parse");
            }
            catch (RequestFailedException ex)
            {
                if (ex.Code == "NEED_PASSWORD")
                {
                    throw new NeedPasswordException(ex.Code, ex);
                }
                else if (ex.Code == "UNSUPPORT_URL")
                {
                    throw new UnsupportUrlException(ex.Code, ex);
                }
                else
                {
                    throw ex;
                }

            }
        }

        public static async Task<OfflineTaskAdd> Add(string savePath, OfflineTaskParameters[] task)
        {
            var data = new { savePath, task };
            return await PostAsync<OfflineTaskAdd>(JsonConvert.SerializeObject(data), "/v3/offline/add");
        }

        public static async Task<OfflineTaskList> GetList(int skip = 1, int limit = 20)
        {
            var data = new { skip, limit };
            return await PostAsync<OfflineTaskList>(JsonConvert.SerializeObject(data), "/v3/offline/list");
        }

        public static async Task<SuccessCount> DeleteTask(string[] taskIdentity, bool deleteFile = false)
        {
            var data = new { taskIdentity, deleteFile };
            return await PostAsync<SuccessCount>(JsonConvert.SerializeObject(data), "/v3/offline/delete");
        }

        public static async Task<DailyQuota> GetQuotaInformation()
        {
            return await PostAsync<DailyQuota>(JsonConvert.SerializeObject(new { }), "/v3/offline/quota");
        }
    }
}
