using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System.Text;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
    public sealed class OfflineDownloader : SixCloudMethodBase
    {


        public static async Task<OfflineTaskParseUrl[]> ParseUrl(string[] urls)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string url in urls)
            {
                stringBuilder.Append(url);
                stringBuilder.Append("\n");
            }
            var data = new { url = stringBuilder.ToString() };
            return await PostAsync<OfflineTaskParseUrl[]>(JsonConvert.SerializeObject(data), "/v2/offline/parseUrl");
        }
        public static async Task<OfflineTaskParseUrl[]> ParseUrl(string url, string password = null)
        {
            var data = password != null ? (object)new { url, password } : new { url };
            return await PostAsync<OfflineTaskParseUrl[]>(JsonConvert.SerializeObject(data), "/v2/offline/parseUrl");
        }
        public static async Task<OfflineTaskParseUrl[]> ParseTorrent(string[] hashs)
        {
            var data = new { hash = hashs };
            return await PostAsync<OfflineTaskParseUrl[]>(JsonConvert.SerializeObject(data), "/v2/offline/parseTorrent");
        }
        public static async Task<OfflineTaskAdd> Add(string path, OfflineTaskParameters[] taskParameters)
        {
            var data = new { path, task = taskParameters };
            return await PostAsync<OfflineTaskAdd>(JsonConvert.SerializeObject(data), "/v2/offline/add");
        }
        public static async Task<OfflineTaskList> GetList(int page = 1, int pageSize = 20)
        {
            var data = new { page, pageSize };
            return await PostAsync<OfflineTaskList>(JsonConvert.SerializeObject(data), "/v2/offline/page");
        }
        public static async Task<int> DeleteTask(string[] identities)
        {
            var data = new { identities };
            return await PostAsync<int>(JsonConvert.SerializeObject(data), "/v2/offline/delete");
        }
    }
}
