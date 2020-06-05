using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SixCloud.Core.Controllers
{
    public class UpdateHelper
    {
        public static async Task<Uri> Check()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            QingzhenyunApis.EntityModels.UpdateInformation x = await QingzhenyunApis.Methods.V4.System.Update(version);
            if (x.Data != default && x.Data.Any())
            {
                System.Collections.Generic.IEnumerable<Uri> query = from package in x.Data
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