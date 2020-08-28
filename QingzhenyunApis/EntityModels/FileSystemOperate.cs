using Newtonsoft.Json;
using QingzhenyunApis.Methods.V3;
using System;

namespace QingzhenyunApis.EntityModels
{
    /// <summary>
    /// 操作文件系统的返回体
    /// </summary>
    public class FileSystemOperate : EntityBodyBase, IDisposable
    {
        private bool disposedValue;

        [JsonProperty(PropertyName = "identity")]
        public string Identity { get; set; }

        [JsonProperty(PropertyName = "data")]
        public int Data { get; set; }

        [JsonProperty(PropertyName = "async")]
        public bool Async { get; set; }

        public event EventHandler<SocketMessageArrivedEventArgs> SocketMessageArrived;

        public void OnSocketMessageArrived(SocketMessage msg)
        {
            SocketMessageArrived?.Invoke(this, new SocketMessageArrivedEventArgs(msg));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    SixCloudMethodBase.FileSystemOperateList.Remove(Identity);
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~FileSystemOperate()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class SocketMessageArrivedEventArgs
    {
        public SocketMessage SocketMessage { get; }

        internal SocketMessageArrivedEventArgs(SocketMessage message)
        {
            SocketMessage = message;
        }
    }
}
