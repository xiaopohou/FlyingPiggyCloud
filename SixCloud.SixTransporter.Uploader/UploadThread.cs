﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace SixCloud.SixTransporter.Uploader
{
    public class UploadThread
    {
        public UploadTaskInfo Info { get; set; }

        public int BlockId { get; set; }

        public UploadBlock BlockInfo => Info.BlockList[BlockId];

        public long ChunkSize { get; set; } = 4 * 1024 * 1024;

        public long LastChunkOffset { get; private set; }

        public string LastChunkCtx { get; private set; }


        public event Action<UploadThread> BlockUploadCompletedEvent;

        public event Action<UploadThread, long> ChunkUploadCompletedEvent;

        private bool _stopped;

        public UploadThread(UploadTaskInfo info, int blockId)
        {
            Info = info;
            BlockId = blockId;
        }
        public void StartUpload()
        {
            Info.BlockList[BlockId].Uploading = true;
            new Thread(CreateBlock) { IsBackground = true }.Start();
        }

        private void CreateBlock()
        {
            if (_stopped)
            {
                return;
            }

            if (BlockInfo.Uploaded)
            {
                BlockUploadCompletedEvent?.Invoke(this);
                return;
            }
            try
            {
                using (FileStream stream = new FileStream(Info.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    stream.Seek(BlockInfo.BeginOffset, SeekOrigin.Begin);
                    byte[] fdata = new byte[ChunkSize];
                    int len = stream.Read(fdata, 0, fdata.Length);
                    if (len < fdata.Length)
                    {
                        byte[] arr = new byte[len];
                        Array.Copy(fdata, arr, len);
                        fdata = arr;
                    }
                    HttpWebRequest request = WebRequest.CreateHttp($"{Info.UploadUrl}/mkblk/{BlockInfo.BlockSize}/{BlockInfo.Id}");
                    request.ContentLength = fdata.Length;
                    request.ContentType = "application/octet-stream";
                    request.Headers.Add("Authorization", Info.Token);
                    request.Headers.Add("UploadBatch", Info.Uuid);
                    request.Method = "POST";
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(fdata, 0, fdata.Length);
                    }

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            string message = reader.ReadToEnd();
                            JObject json = JObject.Parse(message);
                            if (json["code"] != null)
                            {
                                throw new IOException(json.ToString(Formatting.None));
                            }

                            BlockInfo.Ctx = json.Value<string>("ctx");
                            LastChunkCtx = json.Value<string>("ctx");
                            LastChunkOffset = json.Value<long>("offset");
                        }
                    }

                    if (fdata.Length < ChunkSize)
                    {
                        BlockInfo.Uploaded = true;
                        ChunkUploadCompletedEvent?.Invoke(this, fdata.LongLength);
                        BlockUploadCompletedEvent?.Invoke(this);
                        return;
                    }

                    ChunkUploadCompletedEvent?.Invoke(this, ChunkSize);
                    UploadChunk();
                }
            }
            catch (WebException ex)
            {
                Stream stream = ex.Response?.GetResponseStream();
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        try
                        {
                            string message = reader.ReadToEnd();
                            Console.WriteLine(message);
                        }
                        catch (IOException)
                        {
                            //LogHelper.Error("read error stream failed.", ex: ioe);
                        }
                    }
                }

                Console.Write("ERROR: " + ex.Message);
                Exception e = ex.InnerException;
                while (e != null)
                {
                    Console.Write("-> " + e.Message);
                    e = e.InnerException;
                }
                Console.WriteLine();
                new Thread(CreateBlock) { IsBackground = false }.Start();
            }
            catch (Exception)
            {
                new Thread(CreateBlock) { IsBackground = false }.Start();
            }
        }

        private void UploadChunk()
        {
            while (true)
            {
                if (_stopped)
                {
                    return;
                }

                try
                {
                    using (FileStream stream = new FileStream(Info.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        stream.Seek(BlockInfo.BeginOffset + LastChunkOffset, SeekOrigin.Begin);
                        byte[] fdata = new byte[BlockInfo.BeginOffset + LastChunkOffset + ChunkSize > BlockInfo.EndOffset
                            ? BlockInfo.EndOffset - (BlockInfo.BeginOffset + LastChunkOffset)
                            : ChunkSize];
                        int len = stream.Read(fdata, 0, fdata.Length);
                        if (len < fdata.Length)
                        {
                            byte[] arr = new byte[len];
                            Array.Copy(fdata, arr, len);
                            fdata = arr;
                        }

                        //Console.WriteLine(fdata.Length + " : " + LastChunkOffset + " : " + (BlockInfo.End - (BlockInfo.Begin + LastChunkOffset)) + " : " + BlockInfo.Begin + " : " + BlockInfo.End + " : " + (BlockInfo.Begin + LastChunkOffset));
                        if (fdata.Length == 0)
                        {
                            BlockInfo.Uploaded = true;
                            BlockUploadCompletedEvent?.Invoke(this);
                            return;
                        }
                        HttpWebRequest request = WebRequest.CreateHttp($"{Info.UploadUrl}/bput/{LastChunkCtx}/{LastChunkOffset}");
                        request.ContentLength = fdata.Length;
                        request.ContentType = "application/octet-stream";
                        request.Headers.Add("Authorization", Info.Token);
                        request.Headers.Add("UploadBatch", Info.Uuid);
                        request.Method = "POST";
                        using (Stream requestStream = request.GetRequestStream())
                        {
                            requestStream.Write(fdata, 0, fdata.Length);
                        }

                        if (_stopped)
                        {
                            return;
                        }

                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                            {
                                string message = reader.ReadToEnd();
                                JObject json = JObject.Parse(message);
                                if (json["code"] != null)
                                {
                                    throw new IOException(json.ToString(Formatting.None));
                                }

                                LastChunkCtx = json.Value<string>("ctx");
                                BlockInfo.Ctx = json.Value<string>("ctx");
                                //Console.WriteLine("CTX of chunk: "+ json.Value<string>("ctx"));
                                //Console.WriteLine("Offset of chunk: "+json.Value<long>("offset"));
                                LastChunkOffset = json.Value<long>("offset");
                                //LogHelper.Debug($"Upload chunk: - BlockId: {BlockId} BeginOffset: {bOffset} FirstChunkSize: {fdata.Length} NextOffset: {LastChunkOffset} CTX: {LastChunkCtx}");
                                ChunkUploadCompletedEvent?.Invoke(this, fdata.LongLength);
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    Stream stream = ex.Response?.GetResponseStream();
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            try
                            {
                                string message = reader.ReadToEnd();
                                Console.WriteLine(message);
                            }
                            catch (IOException)
                            {
                                //LogHelper.Error("read error stream failed.", ex: ioe);
                            }
                        }
                    }

                    Console.Write("ERROR: " + ex.Message);
                    Exception e = ex.InnerException;
                    while (e != null)
                    {
                        Console.Write("-> " + e.Message);
                        e = e.InnerException;
                    }
                    Console.WriteLine();
                    new Thread(CreateBlock) { IsBackground = true }.Start();
                    break;
                }
                catch (Exception)
                {
                    new Thread(CreateBlock) { IsBackground = true }.Start();
                    break;
                }
            }
        }

        public void Stop()
        {
            _stopped = true;
            BlockInfo.Uploading = false;
        }
    }
}
