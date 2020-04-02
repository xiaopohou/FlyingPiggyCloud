using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
    public sealed class Authentication : SixCloudMethodBase
    {
        public static async Task<GenericResult<UserInformation>> GetUserInformation()
        {
            return await GetAsync<UserInformation>("/v3/user/info");
        }

    }
}