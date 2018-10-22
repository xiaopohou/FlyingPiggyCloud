using System.Threading.Tasks;
using Wangsu.WcsLib.HTTP;

namespace WcsLib.Core
{
    public static class FlyingPiggyCloudUploadMethord
    {
        public static async Task<HttpResult> SimpleUploadAsync(string FilePath, string Token, string UploadUrl)
        {
            Wangsu.WcsLib.Core.SimpleUpload simpleUpload = new Wangsu.WcsLib.Core.SimpleUpload(new Utility.FlyingPiggyClouldAuthToken(Token), UploadUrl);
            HttpResult x = await Task.Run(() =>
                 {
                     return simpleUpload.UploadFile(FilePath);
                 }
            );
            return x;
        }
    }
}
