using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System.Dynamic;
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
        public static async Task<PreviewInformation> Video(string identity)
        {
            var data = new { identity };
            return await PostAsync<PreviewInformation>(JsonConvert.SerializeObject(data), "/v3/preview/video");
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
        /// 获取音频预览地址
        /// </summary>
        /// <param name="identity"></param>
        /// <returns>和Video共用实体类，Height等字段将总是为0</returns>
        public static async Task<PreviewInformation> Audio(string identity)
        {
            var data = new { identity };
            return await PostAsync<PreviewInformation>(JsonConvert.SerializeObject(data), "/v3/preview/audio");
        }

        /// <summary>
        /// 获取图片预览地址
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static async Task<PreviewInformation> Picture(string identity)
        {
            var data = new { identity };
            return await PostAsync<PreviewInformation>(JsonConvert.SerializeObject(data), "/v3/preview/image");
        }

        /// <summary>
        /// 查询文件缩略图
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static async Task<ScreenshotInformation> Screenshot(string identity, int duration)
        {
            var data = new { identity, duration };
            return await PostAsync<ScreenshotInformation>(JsonConvert.SerializeObject(data), "/v3/preview/duration");
        }
    }

    public sealed class Subscribe : SixCloudMethodBase
    {
        /// <summary>
        /// 列出可用的订阅计划
        /// </summary>
        /// <returns></returns>
        public static async Task<DataListResult<SubscriptionPlan>> List()
        {
            return await PostAsync<DataListResult<SubscriptionPlan>>(JsonConvert.SerializeObject(new { }), "/v3/plan/list");
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="planIdentity">订阅计划代码</param>
        /// <returns></returns>
        public static async Task<SubscriptionOrder> Create(int planIdentity)
        {
            return await PostAsync<SubscriptionOrder>(JsonConvert.SerializeObject(new { planIdentity }), "/v3/order/create");
        }

        /// <summary>
        /// 查看订单状态
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static async Task<SubscriptionOrder> CheckOrderStatus(string identity)
        {
            return await PostAsync<SubscriptionOrder>(JsonConvert.SerializeObject(new { identity }), "/v3/order/get");
        }

        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns>默认以创建时间倒叙返回</returns>
        public static async Task<SubscriptionOrder> OrderList(int skip = 0, int limit = 20)
        {
            return await PostAsync<SubscriptionOrder>(JsonConvert.SerializeObject(new { skip, limit }), "/v3/order/list");
        }

    }
}
