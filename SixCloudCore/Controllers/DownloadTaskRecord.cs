using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SixCloudCore.Controllers
{
    internal partial class TasksLogger
    {
        private class DownloadTaskRecord
        {
            public string LocalPath { get; set; }

            public string TargetUUID { get; set; }

            public string Name { get; set; }

            public long BytesReceived { get; set; }

        }
    }

    internal class UpdateHelper
    {
        public static async Task<Uri> Check()
        {
            var x = await QingzhenyunApis.Methods.V4.System.Update(Assembly.GetExecutingAssembly().GetName().Version);
            if (x.Data.Any())
            {
                var query = from package in x.Data
                            orderby package.CreateTime descending
                            select package.DownloadAddress;
                return query.First();
            }
            else
            {
                return default;
            }
        }
    }
}