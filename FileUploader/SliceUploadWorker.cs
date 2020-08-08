using Newtonsoft.Json.Linq;
using SixCloudCore.FileUploader.Calculators;
using SixCloudCore.FileUploader.HTTP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace SixCloudCore.FileUploader
{
    /// <summary>
    /// 这个工人处理分片上传的任务
    /// </summary>
    internal sealed class SliceUploadWorker
    {
        public const int BLOCKSIZE = 4 * 1024 * 1024;

        private const int FIRSTCHUNKSIZE = 1024;

        private readonly ConcurrentQueue<SliceUploadTask> workbook = new ConcurrentQueue<SliceUploadTask>();

        private SliceUploadTask currentTask;

        private readonly HttpManager httpManager;

        /// <summary>
        /// 从等待队列拉取一个激活态的任务
        /// </summary>
        private void PullTask()
        {
            bool isSuccess;
            var waitingTaskCount = 0;
            do
            {
                isSuccess = workbook.TryDequeue(out var nextUploadTask);
                if (isSuccess)
                {
                    switch (nextUploadTask.UploadTaskStatus)
                    {
                        //丢弃和完成的任务移出队列
                        case UploadTaskStatus.Abort:
                        case UploadTaskStatus.Completed:
                            isSuccess = false;
                            break;
                        //暂停和失败任务重新移入队列尾
                        case UploadTaskStatus.Error:
                        case UploadTaskStatus.Pause:
                            isSuccess = false;
                            waitingTaskCount++;
                            workbook.Enqueue(nextUploadTask);
                            break;
                        case UploadTaskStatus.Active:
                            currentTask = nextUploadTask;
                            break;
                    }
                }
            } while (!isSuccess && workbook.Count >= waitingTaskCount);
            if (currentTask.UploadTaskStatus == UploadTaskStatus.Completed || currentTask.UploadTaskStatus == UploadTaskStatus.Abort || currentTask.UploadTaskStatus == UploadTaskStatus.Error)
            {
                currentTask = null;
            }
        }

        /// <summary>
        /// 检查当前任务状态并更新
        /// </summary>
        private void Check()
        {
            if (currentTask == null)
            {
                PullTask();
            }
            else if (currentTask.UploadTaskStatus != UploadTaskStatus.Active)
            {
                switch (currentTask.UploadTaskStatus)
                {
                    case UploadTaskStatus.Abort:
                    case UploadTaskStatus.Completed:
                        PullTask();
                        return;
                    case UploadTaskStatus.Error:
                    case UploadTaskStatus.Pause:
                        workbook.Enqueue(currentTask);
                        PullTask();
                        return;
                }
            }
        }

        private class JobInformation
        {
            public long BlockIndex { get; set; }

            public byte[] Data { get; set; }

            public SliceUploadTask UploadTask { get; set; }

        }

        private IEnumerable<JobInformation> GetJobInformation()
        {
            if (currentTask != null && File.Exists(currentTask.FilePath))
            {
                var task = currentTask;
                var fileStream = new FileStream(task.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var binaryReader = new BinaryReader(fileStream);
                try
                {
                    long blockIndex = 0;
                    do
                    {
                        var data = binaryReader.ReadBytes(BLOCKSIZE);
                        if ((task.TotalContents[blockIndex] == null || task.TotalContents[blockIndex] == "") && task.UploadTaskStatus == UploadTaskStatus.Active)
                        {
                            yield return new JobInformation
                            {
                                BlockIndex = blockIndex,
                                Data = data,
                                UploadTask = task
                            };
                        }
                        else if (task.UploadTaskStatus != UploadTaskStatus.Active)
                        {
                            yield break;
                        }
                        blockIndex++;
                    } while (blockIndex < task.TotalBlockCount);
                }
                finally
                {
                    binaryReader.Close();
                    fileStream.Close();
                }
            }
            else
            {
                yield break;
            }
            yield break;
        }

        private void CarryOn()
        {
            foreach (var jobs in GetJobInformation())
            {
                try
                {
                    if (jobs.BlockIndex == 0)
                    {
                        jobs.UploadTask.TotalContents[jobs.BlockIndex] = UploadFirstBlock(jobs.Data, jobs.BlockIndex, jobs.UploadTask.Token, jobs.UploadTask.Address, jobs.UploadTask.UploadBatch, Path.GetFileName(jobs.UploadTask.FilePath));
                        jobs.UploadTask.CompletedBlockCount++;
#if DEBUG
                        Console.WriteLine("成功上传首个片");
#endif
                    }
                    else
                    {
                        jobs.UploadTask.TotalContents[jobs.BlockIndex] = UploadBlock(jobs.Data, jobs.BlockIndex, jobs.UploadTask.Token, jobs.UploadTask.Address, jobs.UploadTask.UploadBatch, Path.GetFileName(jobs.UploadTask.FilePath));
                        jobs.UploadTask.CompletedBlockCount++;
#if DEBUG
                        Console.WriteLine($"成功上传第{jobs.BlockIndex}个片");
#endif
                    }
                }
                catch (Exception)
                {
                    jobs.UploadTask.UploadTaskStatus = UploadTaskStatus.Pause;
                }
            }
            var task = currentTask;

            if (task.CompletedBlockCount == task.TotalBlockCount)
            {
                foreach (var content in task.TotalContents)
                {
                    if (content == null || content == "")
                    {
                        return;
                    }
                }
                var result = MakeFile(new FileInfo(task.FilePath).Length, Path.GetFileName(task.FilePath), task.TotalContents, task.Token, task.Address, task.UploadBatch);

#if DEBUG
                Console.WriteLine($"成功合成文件{task.FilePath}");
#endif
                var jo = JObject.Parse(result.Text);
                if (jo["hash"].ToString() != ETag.ComputeEtag(task.FilePath))
                {
                    task.UploadTaskStatus = UploadTaskStatus.Error;
#if DEBUG
                    Console.WriteLine("上传校验失败");
#endif
                }
                else
                {
                    task.Hash = jo["hash"].ToString();
                    task.UploadTaskStatus = UploadTaskStatus.Completed;
                }
                return;
            }

            if (task.CompletedBlockCount >= task.TotalBlockCount)
            {
                task.UploadTaskStatus = UploadTaskStatus.Error;
            }
        }

        private void StartWork()
        {
            ThreadPool.QueueUserWorkItem((status) =>
            {
                while (true)
                {
                    if (workbook.Count == 0 && currentTask != null)
                    {
                        Thread.Sleep(500);
                    }
                    else
                    {
                        Check();
                        CarryOn();
                        Thread.Sleep(10);
                    }
                }
            });
        }

        /// <summary>
        /// 上传第一个块
        /// </summary>
        /// <param name="data"></param>
        /// <param name="Index"></param>
        /// <param name="su"></param>
        /// <param name="uploadToken"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        private string UploadFirstBlock(byte[] data, long Index, string uploadToken, string uploadUrl, string uploadBatch, string Key)
        {
            if (data.Length != BLOCKSIZE)
            {
                throw new Exception("文件不足4MB，请使用普通方式上传");
            }

            var result = MakeBlock(BLOCKSIZE, Index, data, 0, FIRSTCHUNKSIZE, uploadToken, uploadUrl, uploadBatch, Key);
            if ((int)HttpStatusCode.OK == result.Code)
            {
                var jo = JObject.Parse(result.Text);
                var ctx = jo["ctx"].ToString();
                // 上传第 1 个 block 剩下的数据
                result = Bput(ctx, FIRSTCHUNKSIZE, data, FIRSTCHUNKSIZE, BLOCKSIZE - FIRSTCHUNKSIZE, uploadToken, uploadUrl, uploadBatch, Key);
                if ((int)HttpStatusCode.OK == result.Code)
                {
                    jo = JObject.Parse(result.Text);
                    return jo["ctx"].ToString();
                }
                else
                {
                    throw new Exception(result.Data.ToString());
                }
            }
            else
            {
                throw new Exception(result.Data.ToString());
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="Index"></param>
        /// <param name="su"></param>
        /// <param name="uploadToken"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        private string UploadBlock(byte[] data, long Index, string uploadToken, string uploadUrl, string uploadBatch, string Key)
        {
            var result = MakeBlock(data.Length, Index, data, 0, data.Length, uploadToken, uploadUrl, uploadBatch, Key);
            if ((int)HttpStatusCode.OK == result.Code)
            {
                var jo = JObject.Parse(result.Text);
                return jo["ctx"].ToString();
            }
            else
            {
                throw new Exception("Exit with error");
            }
        }

        /// <summary>
        /// 创建块(携带首片数据),同时检查CRC32
        /// </summary>
        /// <param name="uploadBatch">分片上传的会话 ID</param>
        /// <param name="blockSize">块大小，除了最后一块可能不足 4MB，前面的所有数据块恒定位 4MB</param>
        /// <param name="blockOrder">块块的顺序号，该序号从 0 开始指定</param>
        /// <param name="chunk">数据片，此操作都会携带第一个数据片</param>
        /// <param name="chunkSize">分片大小，一个块可以被分为若干片依次上传然后拼接或者不分片直接上传整块</param>
        /// <param name="uploadToken">上传凭证</param>
        /// <returns>此操作执行后的返回结果</returns>
        private HttpResult MakeBlock(long blockSize, long blockOrder, byte[] chunk, int chunkOffset, int chunkSize, string uploadToken, string uploadUrl, string uploadBatch, string key = null)
        {
            var url = uploadUrl + "/mkblk/" + blockSize.ToString() + "/" + blockOrder.ToString();
            var customHeaders = new Dictionary<string, string>
            {
                { "UploadBatch", uploadBatch }
            };
            if (!string.IsNullOrEmpty(key))
            {
                customHeaders.Add("Key", Base64.UrlSafeBase64Encode(key));
            }
            return httpManager.PostData(url, chunk, chunkOffset, chunkSize, uploadToken, customHeaders);
        }

        /// <summary>
        /// 上传数据片,同时检查CRC32
        /// </summary>
        /// <param name="uploadBatch">分片上传的会话 ID</param>
        /// <param name="chunk">数据片</param>
        /// <param name="offset">当前片在块中的偏移位置</param>
        /// <param name="chunkSize">当前片的大小</param>
        /// <param name="context">承接前一片数据用到的Context</param>
        /// <param name="uploadToken">上传凭证</param>
        /// <returns>此操作执行后的返回结果</returns>
        private HttpResult Bput(string context, long offset, byte[] chunk, int chunkOffset, int chunkSize, string uploadToken, string uploadUrl, string uploadBatch, string key = null)
        {
            var url = uploadUrl + "/bput/" + context + "/" + offset.ToString();
            var customHeaders = new Dictionary<string, string>
            {
                { "UploadBatch", uploadBatch }
            };
            if (!string.IsNullOrEmpty(key))
            {
                customHeaders.Add("Key", Base64.UrlSafeBase64Encode(key));
            }
            return httpManager.PostData(url, chunk, chunkOffset, chunkSize, uploadToken, customHeaders);
        }

        /// <summary>
        /// 根据已上传的所有分片数据创建文件
        /// </summary>
        /// <param name="uploadBatch">分片上传的会话 ID</param>
        /// <param name="size">文件大小</param>
        /// <param name="key">要保存的文件名</param>
        /// <param name="contexts">所有数据块的Context</param>
        /// <param name="uploadToken">上传凭证</param>
        /// <param name="putExtra">用户指定的额外参数</param>
        /// <returns>此操作执行后的返回结果</returns>
        private HttpResult MakeFile(long size, string key, string[] contexts, string uploadToken, string uploadUrl, string uploadBatch, PutExtra putExtra = null)
        {
            var url = new StringBuilder();
            url.Append(uploadUrl + "/mkfile/" + size.ToString());
            if (null != putExtra && null != putExtra.Params && putExtra.Params.Count > 0)
            {
                foreach (var p in putExtra.Params)
                {
                    if (p.Key.StartsWith("x:"))
                    {
                        url.Append("/" + p.Key + "/" + Base64.UrlSafeBase64Encode(p.Value));
                    }
                }
            }

            var ctxList = new StringBuilder();
            foreach (var ctx in contexts)
            {
                ctxList.Append(ctx + ",");
            }
            var customHeaders = new Dictionary<string, string>
            {
                { "UploadBatch", uploadBatch }
            };
            if (!string.IsNullOrEmpty(key))
            {
                customHeaders.Add("Key", Base64.UrlSafeBase64Encode(key));
            }
            if (null != putExtra && !string.IsNullOrEmpty(putExtra.MimeType))
            {
                customHeaders.Add("MimeType", putExtra.MimeType);
            }
            if (null != putExtra && 0 <= putExtra.Deadline)
            {
                customHeaders.Add("Deadline", putExtra.Deadline.ToString());
            }
            return httpManager.Post(url.ToString(), Encoding.UTF8.GetBytes(ctxList.ToString(0, ctxList.Length - 1)), uploadToken, "text/plain;charset=UTF-8", customHeaders);
        }

        public SliceUploadWorker()
        {
            httpManager = new HttpManager();
            StartWork();
        }

        public void AddTask(SliceUploadTask uploadTask)
        {
            uploadTask.UploadTaskStatus = UploadTaskStatus.Active;
            workbook.Enqueue(uploadTask);
        }
    }
}
