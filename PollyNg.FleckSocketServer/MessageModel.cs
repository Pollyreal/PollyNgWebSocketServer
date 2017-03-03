using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PollyNg.FleckSocketServer
{
    public class MessageModel
    {
        //消息类型
        public Type type{ get; set; }
        //消息文本
        public string content { get; set; }
        //多媒体图片
        public string url { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int userid { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string nickname { get; set; }
    }
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum Type
    {
        /// <summary>
        /// 文本
        /// </summary>
        message,
        /// <summary>
        /// 图像
        /// </summary>
        image,
        /// <summary>
        /// 用户信息
        /// </summary>
        login
    }
}
