using QingzhenyunApis.EntityModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V4
{
    public sealed class System : SixCloudMethodBase
    {
        public static async Task<UpdateInformation> Update(Version version)
        {
#if DEBUG
            var debug = true;
#else
            bool debug = false;
#endif
            return await GetAsync<UpdateInformation>("/v4/system/update", new Dictionary<string, string>
            {
                { "numberVersion",version.Build.ToString()},
                {"platform","desktop-x64" },
                { nameof(debug),debug.ToString()},
                {"lang","zh-CN" }
            }, true);
        }
    }
}
