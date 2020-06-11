using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using System.Threading.Tasks;

namespace QingzhenyunApis.Methods.V3
{
    /// <summary>
    /// 工单
    /// </summary>
    public sealed class Ticket : SixCloudMethodBase
    {
        /// <summary>
        /// 创建工单
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<TicketInformation> Create(string title, string message, TicketType type = TicketType.CommonIssue)
        {
            return await PostAsync<TicketInformation>(JsonConvert.SerializeObject(new { title, message, type }), "/v3/ticket/create");
        }

        /// <summary>
        /// 列出工单
        /// </summary>
        /// <returns></returns>
        public static async Task<DataListResult<TicketInformation>> List()
        {
            return await PostAsync<DataListResult<TicketInformation>>(JsonConvert.SerializeObject(new { }), "/v3/ticket/list");
        }

        /// <summary>
        /// 回复工单
        /// </summary>
        /// <param name="ticketIdentity"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<TicketInformation> Reply(string ticketIdentity, string message)
        {
            return await PostAsync<TicketInformation>(JsonConvert.SerializeObject(new { ticketIdentity, message }), "/v3/ticket/reply");
        }

        /// <summary>
        /// 获取指定工单和回复列表
        /// </summary>
        /// <param name="ticketIdentity"></param>
        /// <returns></returns>
        public static async Task<TicketReplyList> ReplyList(string ticketIdentity)
        {
            return await PostAsync<TicketReplyList>(JsonConvert.SerializeObject(new { ticketIdentity }), "/v3/ticket/reply/list");
        }
    }

}