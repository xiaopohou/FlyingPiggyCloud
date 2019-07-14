using EzWcs.Calculators;
using EzWcs.HTTP;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace EzWcs
{
    internal sealed class SimpleUploadWorker
    {
        private const int MaximumParallelTask = 5;

        private void SimpleUpload(SimpleUploadTask simpleUploadTask)
        {
            try
            {
                HttpResult result = UploadFile(simpleUploadTask.FilePath, simpleUploadTask.Token, simpleUploadTask.Address, Path.GetFileName(simpleUploadTask.FilePath));
                if (result.Code == (int)HttpStatusCode.OK)
                {
                    JObject jo = JObject.Parse(result.Text);
                    simpleUploadTask.Hash = jo["hash"].ToString();
                    simpleUploadTask.UploadTaskStatus = UploadTaskStatus.Completed;
                }
                else
                {
                    simpleUploadTask.UploadTaskStatus = UploadTaskStatus.Error;
                }
            }
            catch (Exception)
            {
                simpleUploadTask.UploadTaskStatus = UploadTaskStatus.Error;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="data">待上传的数据</param>
        /// <param name="key">可选，要保存的key</param>
        /// <param name="extra">可选，上传可选设置</param>
        /// <returns>上传数据后的返回结果</returns>
        private HttpResult UploadData(byte[] data, string token, string uploadUrl, string key = null, PutExtra putExtra = null)
        {

            if (putExtra == null)
            {
                putExtra = new PutExtra();
            }

            string filename = key;
            if (string.IsNullOrEmpty(key))
            {
                filename = "uploading.tmp";
            }

            string boundary = HttpManager.CreateFormDataBoundary();
            StringBuilder bodyBuilder = new StringBuilder();
            bodyBuilder.Append("--" + boundary + "\r\n"
                + "Content-Disposition: form-data; name=\"token\"\r\n\r\n"
                + token + "\r\n");
            if (null != putExtra.Params && putExtra.Params.Count > 0)
            {
                foreach (KeyValuePair<string, string> p in putExtra.Params)
                {
                    if (p.Key.StartsWith("x:"))
                    {
                        bodyBuilder.Append("--" + boundary + "\r\n"
                            + "Content-Disposition: form-data; name=\"" + p.Key + "\"\r\n\r\n"
                            + p.Value + "\r\n");
                    }
                }
            }

            // write key
            if (null != key)
            {
                bodyBuilder.Append("--" + boundary + "\r\n"
                    + "Content-Disposition: form-data; name=\"key\"\r\n\r\n"
                    + key + "\r\n");
            }

            // write mime type
            if (!string.IsNullOrEmpty(putExtra.MimeType))
            {
                bodyBuilder.Append("--" + boundary + "\r\n"
                    + "Content-Disposition: form-data; name=\"mimeType\"\r\n\r\n"
                    + putExtra.MimeType + "\r\n");
            }

            // write deadline
            if (-1 != putExtra.Deadline)
            {
                bodyBuilder.Append("--" + boundary + "\r\n"
                    + "Content-Disposition: form-data; name=\"deadline\"\r\n\r\n"
                    + putExtra.Deadline.ToString() + "\r\n");
            }

            // write filename
            bodyBuilder.Append("--" + boundary + "\r\n"
                + "Content-Disposition: form-data; name=\"file\"; filename=\""
                + filename + "\"\r\nContent-Type: application/octet-stream\r\n\r\n");

            // write file data
            StringBuilder bodyEnd = new StringBuilder();
            bodyEnd.Append("\r\n--" + boundary + "--\r\n");

            byte[] partHead = Encoding.UTF8.GetBytes(bodyBuilder.ToString());
            byte[] partTail = Encoding.UTF8.GetBytes(bodyEnd.ToString());

            // 允许空内容
            int dataLength = 0;
            if (null != data)
            {
                dataLength = data.Length;
            }

            byte[] body = new byte[partHead.Length + dataLength + partTail.Length];
            //Array.Copy(partHead, 0, body, 0, partHead.Length);
            // Buffer.BlockCopy 比 Array.Copy 简单，所以更快。
            Buffer.BlockCopy(partHead, 0, body, 0, partHead.Length);
            if (null == data)
            {
                Buffer.BlockCopy(partTail, 0, body, partHead.Length, partTail.Length);
            }
            else
            {
                Buffer.BlockCopy(data, 0, body, partHead.Length, data.Length);
                Buffer.BlockCopy(partTail, 0, body, partHead.Length + data.Length, partTail.Length);
            }
            string url = uploadUrl + "/file/upload";
            HttpResult result = httpManager.PostMultipart(url, body, boundary);

            return result;
        }

        /// <summary>
        /// 上传数据流
        /// </summary>
        /// <param name="stream">(确定长度的)数据流</param>
        /// <param name="putPolicy">上传策略数据，JSON 字符串</param>
        /// <param name="key">可选，要保存的key</param>
        /// <param name="extra">上传可选设置</param>
        /// <returns>上传数据流后的返回结果</returns>
        private HttpResult UploadStream(Stream stream, string token, string uploadUrl, string key = null, PutExtra putExtra = null)
        {
            int bufferSize = 4 * 1024 * 1024;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            using (MemoryStream dataMS = new MemoryStream())
            {
                while ((bytesRead = stream.Read(buffer, 0, bufferSize)) != 0)
                {
                    dataMS.Write(buffer, 0, bytesRead);
                }
                return UploadData(dataMS.ToArray(), token, uploadUrl, key, putExtra);
            }
        }

        /// <summary>
        /// 上传本地文件，可能抛异常
        /// </summary>
        /// <param name="localFilename">本地文件名</param>
        /// <param name="putPolicy">上传策略数据，JSON 字符串</param>
        /// <param name="key">可选，要保存的key</param>
        /// <param name="extra">上传可选设置</param>
        /// <returns>上传数据流后的返回结果</returns>
        private HttpResult UploadFile(string localFilename, string token, string uploadUrl, string key = null, PutExtra putExtra = null)
        {
            using (FileStream fs = new FileStream(localFilename, FileMode.Open))
            {
                return UploadStream(fs, token, uploadUrl, key, putExtra);
            }
        }

        private HttpManager httpManager;

        private ConcurrentQueue<SimpleUploadTask> workbook = new ConcurrentQueue<SimpleUploadTask>();

        private SimpleUploadTask[] runningTasks = new SimpleUploadTask[MaximumParallelTask];

        private void Check()
        {
            List<int> completedIndex = new List<int>();
            for (int i = 0; i < runningTasks.Length; i++)
            {
                if (runningTasks[i] == null || runningTasks[i].UploadTaskStatus != UploadTaskStatus.Active)
                {
                    completedIndex.Add(i);
                }
            }
            for (int i = 0; i < workbook.Count && i < completedIndex.Count; i++)
            {
                bool isSuccess = false;
                do
                {
                    if (!workbook.TryDequeue(out SimpleUploadTask task))
                    {
                        break;
                    }

                    if (task.UploadTaskStatus == UploadTaskStatus.Active)
                    {
                        runningTasks[completedIndex[i]] = task;
#if DEBUG
                        ThreadPool.QueueUserWorkItem((state) =>
                        {
                            Console.WriteLine("开始上传简单任务");
                            SimpleUpload(task);
                            Console.WriteLine("上传结束");
                        });
#else
                        ThreadPool.QueueUserWorkItem((state) => SimpleUpload(task));
#endif
                    }
                } while (!isSuccess);
            }
        }

        private void StartWork()
        {
            ThreadPool.QueueUserWorkItem((status) =>
            {
                while (true)
                {
                    if (workbook.Count == 0)
                    {
                        Thread.Sleep(500);
                    }
                    else
                    {
                        Check();
                        Thread.Sleep(10);
                    }
                }
            });
        }

        public void AddTask(SimpleUploadTask simpleUploadTask)
        {
            workbook.Enqueue(simpleUploadTask);
        }

        public SimpleUploadWorker()
        {
            httpManager = new HttpManager();
            StartWork();
        }
    }
}
