using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Wangsu.WcsLib.HTTP;
using Wangsu.WcsLib.Utility;
using WcsLib.Core;
using WcsLib.Exception;
using WcsLib.Utility;

namespace Wangsu.WcsLib.Core
{
    public delegate void UserCommandEventHandle(object sender, EventArgs e);

    public static class Upload
    {
        private const int BLOCKSIZE = 4 * 1024 * 1024;
        private const int FIRSTCHUNKSIZE = 1024;

        /// <summary>
        /// 启动一个上传任务
        /// </summary>
        /// <param name="UploadToken">Qingzhenyun返回的上传token</param>
        /// <param name="FilePath">文件的本地路径</param>
        /// <param name="UploadUrl">Qingzhenyun返回的上传地址</param>
        public static void Start(string UploadToken, string FilePath, string UploadUrl,string Key=null, UploadProgressHandler uploadProgressHandler=null, UploadTaskOperator userCommand=null)
        {
            Config config = new Config(UploadUrl);
            string eTag = ETag.ComputeEtag(FilePath);
            long dataSize = new FileInfo(FilePath).Length;
            uploadProgressHandler?.Invoke(0, dataSize);
            if (dataSize < BLOCKSIZE)
            {
                SimpleUpload(FilePath, UploadToken, UploadUrl);
                uploadProgressHandler?.Invoke(dataSize, dataSize);
            }
            else
            {
                FileStream fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                try
                {
                    userCommand.userCommand?.Invoke();
                    long blockCount = (dataSize + BLOCKSIZE - 1) / BLOCKSIZE;
                    string[] TotalContexts = new string[blockCount];
                    // 第一个分片不宜太大，因为可能遇到错误，上传太大是白费流量和时间！
                    SliceUpload su = new SliceUpload(config);
                    long Index = 0;
                    TotalContexts[Index] = UploadFirstBlock(binaryReader.ReadBytes(BLOCKSIZE), Index, su, UploadToken, Key);
                    uploadProgressHandler?.Invoke(Index * BLOCKSIZE, dataSize);
                    do
                    {
                        userCommand.userCommand?.Invoke();
                        Index++;
                        TotalContexts[Index] = UploadBlock(binaryReader.ReadBytes(BLOCKSIZE), Index, su, UploadToken, Key);
                        uploadProgressHandler?.Invoke(Index * BLOCKSIZE < dataSize ? Index * BLOCKSIZE : dataSize, dataSize);
                    } while (Index < blockCount - 1);
                    //上传结束，将所有的块合成一个文件
                    HttpResult result = su.MakeFile(dataSize, null, TotalContexts, UploadToken);
                    JObject jo = JObject.Parse(result.Text);
                    uploadProgressHandler?.Invoke(dataSize, dataSize);
                    if (jo["hash"].ToString() != eTag)
                        throw new Exception("上传文件校验失败");
                    //Console.WriteLine("---MakeFile---\n{0}", result.ToString());
                }
                catch(OperatingAbortedException)
                {
                    throw new Exception("上传任务被用户取消");
                }
                finally
                {
                    binaryReader.Close();
                }
            }
        }

        private static string UploadBlock(byte[] data,long Index, SliceUpload su, string uploadToken,string Key)
        {
            HttpResult result = su.MakeBlock(data.Length, Index, data, 0, data.Length, uploadToken,Key);
            if ((int)HttpStatusCode.OK == result.Code)
            {
                JObject jo = JObject.Parse(result.Text);
                return jo["ctx"].ToString();
            }
            else
            {
                throw new Exception("Exit with error");
            }
        }

        private static string UploadFirstBlock(byte[] data,long Index, SliceUpload su, string uploadToken,string Key)
        {
            if(data.Length != BLOCKSIZE)
            {
                throw new Exception("文件不足4MB，请使用普通方式上传");
            }

            HttpResult result = su.MakeBlock(BLOCKSIZE, Index, data, 0, FIRSTCHUNKSIZE, uploadToken,Key);
            if ((int)HttpStatusCode.OK == result.Code)
            {
                JObject jo = JObject.Parse(result.Text);
                string ctx = jo["ctx"].ToString();
                // 上传第 1 个 block 剩下的数据
                result = su.Bput(ctx, FIRSTCHUNKSIZE, data, FIRSTCHUNKSIZE, BLOCKSIZE - FIRSTCHUNKSIZE, uploadToken,Key);
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

        private static async Task<HttpResult> SimpleUploadAsync(string FilePath, string Token, string UploadUrl)
        {
            SimpleUpload simpleUpload = new SimpleUpload(Token, new Config(UploadUrl));
            HttpResult x = await Task.Run(() =>
            {
                return simpleUpload.UploadFile(FilePath);
            }
            );
            return x;
        }

        private static HttpResult SimpleUpload(string FilePath, string Token, string UploadUrl)
        {
            SimpleUpload simpleUpload = new SimpleUpload(Token, new Config(UploadUrl));
            return simpleUpload.UploadFile(FilePath);
        }
    }
}
