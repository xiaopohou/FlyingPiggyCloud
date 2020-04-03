using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System.Text;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods
{
    //public sealed class OfflineDownloader : SixCloudMethordBase
    //{
    //    public async Task<GenericResult<OfflineTaskParseUrl[]>> ParseUrl(string[] urls)
    //    {
    //        StringBuilder stringBuilder = new StringBuilder();
    //        foreach (string url in urls)
    //        {
    //            stringBuilder.Append(url);
    //            stringBuilder.Append("\n");
    //        }
    //        var data = new { url = stringBuilder.ToString() };
    //        return await PostAsync<GenericResult<OfflineTaskParseUrl[]>>(JsonConvert.SerializeObject(data), "/v2/offline/parseUrl", false);
    //    }

    //    public async Task<GenericResult<OfflineTaskParseUrl[]>> ParseUrl(string url, string password = null)
    //    {
    //        var data = password != null ? (object)new { url, password } : new { url };
    //        return await PostAsync<GenericResult<OfflineTaskParseUrl[]>>(JsonConvert.SerializeObject(data), "/v2/offline/parseUrl", false);
    //    }


    //    public async Task<GenericResult<OfflineTaskParseUrl[]>> ParseTorrent(string[] hashs)
    //    {
    //        var data = new { hash = hashs };
    //        return await PostAsync<GenericResult<OfflineTaskParseUrl[]>>(JsonConvert.SerializeObject(data), "/v2/offline/parseTorrent", false);
    //    }

    //    public async Task<GenericResult<OfflineTaskAdd>> Add(string path, OfflineTaskParameters[] taskParameters)
    //    {
    //        var data = new { path, task = taskParameters };
    //        return await PostAsync<GenericResult<OfflineTaskAdd>>(JsonConvert.SerializeObject(data), "/v2/offline/add", false);
    //    }

    //    public async Task<GenericResult<OfflineTaskList>> GetList(int page = 1, int pageSize = 20)
    //    {
    //        var data = new { page, pageSize };
    //        return await PostAsync<GenericResult<OfflineTaskList>>(JsonConvert.SerializeObject(data), "/v2/offline/page", false);
    //    }

    //    public async Task<GenericResult<int>> DeleteTask(string[] identities)
    //    {
    //        var data = new { identities };
    //        return await PostAsync<GenericResult<int>>(JsonConvert.SerializeObject(data), "/v2/offline/delete", false);
    //    }
    //}
}
