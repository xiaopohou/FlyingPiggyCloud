using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
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
