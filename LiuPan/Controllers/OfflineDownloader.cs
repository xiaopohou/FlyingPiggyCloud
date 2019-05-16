using Newtonsoft.Json;
using SixCloud.Models;
using SixCloud.Views;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SixCloud.Controllers
{
    internal sealed class OfflineDownloader : SixCloudMethordBase
    {
        public GenericResult<OfflineTaskParseUrl[]> ParseUrl(string[] urls)
        {
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string url in urls)
            {
                stringBuilder.Append(url);
                stringBuilder.Append("\n");
            }
            var data = new { url = stringBuilder.ToString() };
            GenericResult<OfflineTaskParseUrl[]> x = Post<GenericResult<OfflineTaskParseUrl[]>>(JsonConvert.SerializeObject(data), "v2/offline/parseUrl", new Dictionary<string, string>
            {
                { "Qingzhen-Token",Token }
            }, out WebHeaderCollection webHeaderCollection);
            Token = webHeaderCollection.Get("qingzhen-token");
            return x;
        }

        public GenericResult<OfflineTaskAdd> Add(string path, OfflineTaskParameters[] taskParameters)
        {
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            var data = new { path, task = taskParameters };
            GenericResult<OfflineTaskAdd> x = Post<GenericResult<OfflineTaskAdd>>(JsonConvert.SerializeObject(data), "v2/offline/add", new Dictionary<string, string>
            {
                { "Qingzhen-Token",Token }
            }, out WebHeaderCollection webHeaderCollection);
            Token = webHeaderCollection.Get("qingzhen-token");
            return x;
        }

        public GenericResult<OfflineTaskList> GetList(int page = 1, int pageSize = 20)
        {
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            var data = new { page, pageSize };
            GenericResult<OfflineTaskList> x = Post<GenericResult<OfflineTaskList>>(JsonConvert.SerializeObject(data), "v2/offline/page", new Dictionary<string, string>
            {
                { "Qingzhen-Token",Token }
            }, out WebHeaderCollection webHeaderCollection);
            Token = webHeaderCollection.Get("qingzhen-token");
            return x;
        }

        public GenericResult<OfflineTaskList> DeleteTask(string[] identities)
        {
            while (string.IsNullOrWhiteSpace(Token))
            {
                LoginView GetToken = new LoginView();
                GetToken.ShowDialog();
            }
            var data = new { identities };
            GenericResult<OfflineTaskList> x = Post<GenericResult<OfflineTaskList>>(JsonConvert.SerializeObject(data), "v2/offline/delete", new Dictionary<string, string>
            {
                { "Qingzhen-Token",Token }
            }, out WebHeaderCollection webHeaderCollection);
            Token = webHeaderCollection.Get("qingzhen-token");
            return x;
        }
    }
}
