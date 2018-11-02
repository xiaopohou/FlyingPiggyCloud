using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FlyingPiggyCloud.Controllers
{
    /// <summary>
    /// APP生命周期内，所有派生于此抽象类的对象，均拥有共同的Token字段
    /// </summary>
    public abstract class QingzhenyunRequestBase
    {
        protected RestClient client;

        protected static string Token;

        protected async Task<T> PostAsync<T>(string data, string uri) => JsonConvert.DeserializeObject<T>(await client.PostAsync(data, uri));

        protected T Get<T>(string uri)
        {
            return JsonConvert.DeserializeObject<T>(client.Get(uri));
        }

        public QingzhenyunRequestBase(string BaseUri)
        {
            client = new RestClient(BaseUri);
            if (RegistryManager.IsAutoLogin && Token == null)
            {
                Token = RegistryManager.Token;
            }
        }

        ~QingzhenyunRequestBase()
        {
            if (RegistryManager.IsAutoLogin && Token != null)
            {
                RegistryManager.Token = Token;
            }
        }
    }
}