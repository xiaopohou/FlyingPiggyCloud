using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System.Dynamic;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
    public sealed class RecoveryBox : SixCloudMethodBase
    {
        /// <summary>
        /// 清空当前用户的回收站
        /// </summary>
        /// <returns></returns>
        public static async Task<SuccessCount> Empty()
        {
            return await PostAsync<SuccessCount>(JsonConvert.SerializeObject(new object()), "/v3/trash/clear");
        }

        /// <summary>
        /// 从回收站恢复文件
        /// </summary>
        /// <param name="targetUUID">待恢复的一组文件或文件夹</param>
        /// <param name="parentUUID">恢复到其他文件夹</param>
        /// <param name="parentPath">恢复到其他路径</param>
        /// <returns></returns>
        public static async Task<SuccessCount> Restore(string[] sourceIdentity, string parentUUID = null, string parentPath = null)
        {
            dynamic data = new ExpandoObject();
            data.sourceIdentity = sourceIdentity;
            if (parentUUID != null)
            {
                data.identity = parentUUID;
            }
            else if (parentPath != null)
            {
                data.path = parentPath;
            }
            return await PostAsync<SuccessCount>(JsonConvert.SerializeObject(data), "/v3/trash/recover");
        }

        public static async Task<RecoveryBoxPage> GetList(int skip = 0, int limit = 20)
        {
            var data = new { skip, limit };
            return await PostAsync<RecoveryBoxPage>(JsonConvert.SerializeObject(data), "/v3/trashCan/list/");
        }

        /// <summary>
        /// 从回收站删除文件
        /// </summary>
        /// <param name="targetUUID"></param>
        /// <returns></returns>
        public static async Task<SuccessCount> Delete(string[] targetUUID)
        {
            return await PostAsync<SuccessCount>(JsonConvert.SerializeObject(new { sourceIdentity = targetUUID }), "/v3/trash/delete");
        }
    }
}
