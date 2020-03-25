using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SixCloudCore.Controllers
{
    internal static class SingleInstanceManager
    {
        private static Mutex mutex;

        private static NamedPipeServerStream pipeServer;

        private static void CreateMonitor()
        {
            if (pipeServer != null)
            {
                ThreadPool.QueueUserWorkItem((_) =>
                {
                    do
                    {
                        pipeServer.WaitForConnection();
                        using StreamReader reader = new StreamReader(pipeServer);
                        var newMessage = JsonConvert.DeserializeObject<IList<string>>(reader.ReadLine());
                        NewMessage?.Invoke(new CrossProcessMessageEventArgs
                        {
                            Message = newMessage
                        }); ;
                    } while (true);
                });
            }
        }

        /// <summary>
        /// 尝试获取SixCloudWPF互斥锁
        /// </summary>
        /// <returns>如果为真，则当前无其他6盘客户端进程</returns>
        internal static bool Check()
        {
            mutex = new Mutex(true, "SixCloudWPF");
            if (mutex.WaitOne(0, false))
            {
                pipeServer = new NamedPipeServerStream("SixCloud", PipeDirection.In);
                CreateMonitor();
                return true;
            }
            else
            {
                using NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream("localhost", "SixCloud", PipeDirection.Out);
                namedPipeClientStream.Connect(200);
                using StreamWriter writer = new StreamWriter(namedPipeClientStream);
                writer.WriteLine(JsonConvert.SerializeObject(Environment.GetCommandLineArgs()));
                return false;
            }
        }

        #region event
        internal static event CrossProcessMessageHandler NewMessage;

        internal delegate void CrossProcessMessageHandler(CrossProcessMessageEventArgs e);

        internal class CrossProcessMessageEventArgs : EventArgs
        {
            internal IList<string> Message { get; set; }
        }
        #endregion
    }

}
