using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SixCloudCore.Controllers
{
    internal class UpdateHelper
    {
        public static async Task<Uri> Check()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var x = await QingzhenyunApis.Methods.V4.System.Update(version);
            if (x.Data != default && x.Data.Any())
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