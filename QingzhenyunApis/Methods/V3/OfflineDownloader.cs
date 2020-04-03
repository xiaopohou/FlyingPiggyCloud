using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System.Text;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
    public sealed class OfflineDownloader : SixCloudMethodBase
    {
        public async Task<GenericResult<OfflineTaskParseUrl[]>> ParseUrl(string[] urls)
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

        public async Task<GenericResult<OfflineTaskParseUrl[]>> ParseUrl(string url, string password = null)
        {
            var data = password != null ? (object)new { url, password } : new { url };
            return await PostAsync<OfflineTaskParseUrl[]>(JsonConvert.SerializeObject(data), "/v2/offline/parseUrl");
        }


        public async Task<GenericResult<OfflineTaskParseUrl[]>> ParseTorrent(string[] hashs)
        {
            var data = new { hash = hashs };
            return await PostAsync<OfflineTaskParseUrl[]>(JsonConvert.SerializeObject(data), "/v2/offline/parseTorrent");
        }

        public async Task<GenericResult<OfflineTaskAdd>> Add(string path, OfflineTaskParameters[] taskParameters)
        {
            var data = new { path, task = taskParameters };
            return await PostAsync<OfflineTaskAdd>(JsonConvert.SerializeObject(data), "/v2/offline/add");
        }

        public async Task<GenericResult<OfflineTaskList>> GetList(int page = 1, int pageSize = 20)
        {
            var data = new { page, pageSize };
            return await PostAsync<OfflineTaskList>(JsonConvert.SerializeObject(data), "/v2/offline/page");
        }

        public async Task<GenericResult<int>> DeleteTask(string[] identities)
        {
            var data = new { identities };
            return await PostAsync<int>(JsonConvert.SerializeObject(data), "/v2/offline/delete");
        }
    }
}
