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
        public static async Task<int?> Empty()
        {
            return await PostAsync<int?>(JsonConvert.SerializeObject(new object()), "/v2/trash/truncate");
        }

        /// <summary>
        /// 从回收站恢复文件
        /// </summary>
        /// <param name="targetUUID">待恢复的一组文件或文件夹</param>
        /// <param name="parentUUID">恢复到其他文件夹</param>
        /// <param name="parentPath">恢复到其他路径</param>
        /// <returns></returns>
        public static async Task<int?> Restore(string[] targetUUID, string parentUUID = null, string parentPath = null)
        {
            dynamic data = new ExpandoObject();
            data.source = targetUUID;
            if (parentUUID != null)
            {
                data.identity = parentUUID;
            }
            else if (parentPath != null)
            {
                data.path = parentPath;
            }
            return await PostAsync<int?>(JsonConvert.SerializeObject(data), "/v2/trash/moveFromTrash");
        }

        /// <summary>
        /// 从回收站删除文件
        /// </summary>
        /// <param name="targetUUID"></param>
        /// <returns></returns>
        public static async Task<int?> Delete(string[] targetUUID)
        {
            return await PostAsync<int?>(JsonConvert.SerializeObject(targetUUID), "/v2/trash/delete");
        }

        public static async Task<RecoveryBoxPage> GetList(int start = 0, int limit = 20)
        {
            var data = new { start, limit };
            return await PostAsync<RecoveryBoxPage>(JsonConvert.SerializeObject(data), "/v3/trashCan/list/");
        }
    }
}
