using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
    public sealed class Preview : SixCloudMethodBase
    {
        /// <summary>
        /// 获取视频预览地址
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static async Task<PreviewVideoInformation> Video(string identity)
        {
            var data = new { identity };
            return await PostAsync<PreviewVideoInformation>(JsonConvert.SerializeObject(data), "/v3/preview/video");
        }

        /// <summary>
        /// 查询视频预览字幕
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static async Task<PreviewVideoSubtitleList> VideoSubtitle(string identity)
        {
            var data = new { identity };
            return await PostAsync<PreviewVideoSubtitleList>(JsonConvert.SerializeObject(data), "/v3/subtitle/get");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <returns>和Video共用实体类，Height等字段将总是为0</returns>
        public static async Task<PreviewVideoInformation> Audio(string identity)
        {
            var data = new { identity };
            return await PostAsync<PreviewVideoInformation>(JsonConvert.SerializeObject(data), "/v3/preview/audio");
        }
    }
}
