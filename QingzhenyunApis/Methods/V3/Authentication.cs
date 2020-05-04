using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
    public sealed class Authentication : SixCloudMethodBase
    {
        public static async Task<UserInformation> GetUserInformation(string token = null)
        {
            Token ??= token;
            return await GetAsync<UserInformation>("/v3/user/info");
        }

#warning 此接口行为尚未验证
        public static async Task Logout()
        {
            await PostAsync<object>("", "/v3/user/logout");
            Token = null;
        }

        /// <summary>
        /// 获取登陆令牌
        /// </summary>
        /// <returns></returns>
        public static async Task<DestinationInformation> CreateDestination()
        {
            var data = new { ts = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds };
            return await PostAsync<DestinationInformation>(JsonConvert.SerializeObject(data), "/v3/user/createDestination", isAnonymous: true);
        }

        /// <summary>
        /// 获取登陆页面地址
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string GetLoginUrl(string destination, out string state)
        {
            state = Guid.NewGuid().ToString();
            return $"https://account.6pan.cn/login?destination={destination}&appid={AccessKeyId}&response=query&state={state}&lang=zh-CN";
        }

        /// <summary>
        /// 检查登陆结果
        /// </summary>
        /// <param name="destinationInfo"></param>
        /// <returns></returns>
        public static async Task<bool> CheckDestination(DestinationInformation destinationInfo)
        {
            var data = new { destination = destinationInfo.Destination };
            TokenInformation x;
            do
            {
                Thread.Sleep(100);
                x = await PostAsync<TokenInformation>(JsonConvert.SerializeObject(data), "/v3/user/checkDestination", isAnonymous: true);
            } while (x.Status == 10);
            if (x.Status == 100)
            {
                Token = x.Token;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<UserInformation> ChangeUserName(string name)
        {
            return await PatchAsync<UserInformation>(JsonConvert.SerializeObject(new { name }), "/v3/user/update");
        }

        /// <summary>
        /// 修改密码，传入值必须为md5
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public static async Task<UserInformation> ChangePassword(string oldPassword, string newPassword)
        {
            return await PostAsync<UserInformation>(JsonConvert.SerializeObject(new { oldPassword, newPassword }), "/v3/user/changePassword");
        }

    }
}