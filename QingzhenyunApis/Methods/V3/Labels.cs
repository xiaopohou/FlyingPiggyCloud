using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
    public class Labels : SixCloudMethodBase
    {
        public static async Task<Label> Create(int identity, string name)
        {
            return await PostAsync<Label>(JsonConvert.SerializeObject(new { identity, name }), "/v3/labels");
        }

        public static async Task<DataListResult<Label>> List(int skip, int limit)
        {
            return await PostAsync<DataListResult<Label>>(JsonConvert.SerializeObject(new { skip, limit }), "/v3/labels");
        }

        public static async Task<Label> ModifyTag(int identity, string name)
        {
            return await PutAsync<Label>(JsonConvert.SerializeObject(new { name }), $"/v3/labels/{identity}");
        }

        public static async Task<FileSystemOperate> Remove(string[] sourceIdentity)
        {
            var data = new { sourceIdentity };

            return await PostAsync<FileSystemOperate>(JsonConvert.SerializeObject(data), "/v3/labels/delete");
        }

        public static async Task<FileSystemOperate> SetLabel(string[] sourceIdentity, int label)
        {
            var data = new { sourceIdentity, label };

            return await PostAsync<FileSystemOperate>(JsonConvert.SerializeObject(data), "/v3/newfile/addLabel");
        }

        public static async Task<FileSystemOperate> RemoveLabel(string[] sourceIdentity)
        {
            var data = new { sourceIdentity };

            return await PostAsync<FileSystemOperate>(JsonConvert.SerializeObject(data), "/v3/newfile/removeLabel");
        }

    }
}
