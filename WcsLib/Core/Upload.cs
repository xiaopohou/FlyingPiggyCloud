using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Wangsu.WcsLib.HTTP;
using WcsLib.Utility;

namespace Wangsu.WcsLib.Core
{
    public static class Upload
    {
        private const int BLOCKSIZE = 4 * 1024 * 1024;
        private const int FIRSTCHUNKSIZE = 1024;

        public static void Start(string uploadToken, string FilePath, string UploadUrl)
        {
            Config config = new Config(UploadUrl);
            long dataSize = new FileInfo(FilePath).Length;
            if (dataSize < BLOCKSIZE)
                throw new Exception("文件过小，请使用simpleUpload上传");
            FileStream fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            
            long blockCount = (dataSize + BLOCKSIZE - 1) / BLOCKSIZE;
            string[] TotalContexts = new string[blockCount];
            //一次只读取相当于一个块长度的数据
            //byte[] data = binaryReader.ReadBytes(BLOCKSIZE);

            // 最后合成文件时的 hash
            //Console.WriteLine("ETag of uploading data: {0}", ETag.ComputeEtag(data));

            // 第一个分片不宜太大，因为可能遇到错误，上传太大是白费流量和时间！
            SliceUpload su = new SliceUpload(config);
            long Index = 0;
            TotalContexts[Index] = UploadFirstBlock(binaryReader.ReadBytes(BLOCKSIZE), Index, su, uploadToken);
            do
            {
                Index++;
                TotalContexts[Index] = UploadBlock(binaryReader.ReadBytes(BLOCKSIZE), Index, su, uploadToken);
            } while (Index == blockCount);
            //上传结束，将所有的块合成一个文件
            HttpResult result = su.MakeFile(dataSize, null, TotalContexts, uploadToken);
            Console.WriteLine("---MakeFile---\n{0}", result.ToString());
        }

        private static string UploadBlock(byte[] data,long Index, SliceUpload su, string uploadToken)
        {
            HttpResult result = su.MakeBlock(data.Length, Index, data, 0, data.Length, uploadToken);
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

        private static string UploadFirstBlock(byte[] data,long Index, SliceUpload su, string uploadToken)
        {
            if(data.Length != BLOCKSIZE)
            {
                throw new Exception("文件不足4MB，请使用普通方式上传");
            }

            HttpResult result = su.MakeBlock(BLOCKSIZE, Index, data, 0, FIRSTCHUNKSIZE, uploadToken);
            if ((int)HttpStatusCode.OK == result.Code)
            {
                JObject jo = JObject.Parse(result.Text);
                string ctx = jo["ctx"].ToString();
                // 上传第 1 个 block 剩下的数据
                result = su.Bput(ctx, FIRSTCHUNKSIZE, data, FIRSTCHUNKSIZE, BLOCKSIZE - FIRSTCHUNKSIZE, uploadToken);
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
            SimpleUpload simpleUpload = new SimpleUpload(new Auth(Token), new Config(UploadUrl));
            HttpResult x = await Task.Run(() =>
            {
                return simpleUpload.UploadFile(FilePath);
            }
            );
            return x;
        }
    }
}
