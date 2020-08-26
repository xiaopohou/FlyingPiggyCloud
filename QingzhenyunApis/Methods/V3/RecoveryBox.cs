using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System.Dynamic;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
    public sealed class RecoveryBox : SixCloudMethodBase
    {
        public static async Task<DataListResult<RecoveryBoxItem>> GetList(long skip = 0, long limit = 20)
        {
            var data = new { skip, limit };
            return await PostAsync<DataListResult<RecoveryBoxItem>>(JsonConvert.SerializeObject(data), "/v3/newtrash/list");
        }

        /// <summary>
        /// 从回收站恢复文件，如果 path identity 均为空，则恢复到原始位置
        /// </summary>
        /// <param name="sourceIdentity">回收id</param>
        /// <param name="parentUUID">目的地id，与path二选一</param>
        /// <param name="parentPath">目的地路径，与identity二选一</param>
        /// <returns></returns>
        public static async Task<FileSystemOperate> Restore(string[] sourceIdentity, string parentUUID = null, string parentPath = null)
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

            return await PostAsync<FileSystemOperate>(JsonConvert.SerializeObject(data), "/v3/newtrash/recover");
        }

        /// <summary>
        /// 从回收站删除文件
        /// </summary>
        /// <param name="targetUUID"></param>
        /// <returns></returns>
        public static async Task<FileSystemOperate> Delete(string[] sourceIdentity)
        {
            return await PostAsync<FileSystemOperate>(JsonConvert.SerializeObject(new { sourceIdentity }), "/v3/newtrash/delete");
        }

        /// <summary>
        /// 清空当前用户的回收站
        /// </summary>
        /// <returns></returns>
        public static async Task<FileSystemOperate> Empty()
        {
            return await PostAsync<FileSystemOperate>(JsonConvert.SerializeObject(new object()), "/v3/newtrash/clear");
        }


    }
}
