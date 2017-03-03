using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PollyNg.API.Service
{
    /// <summary>
    /// OSS签名模型
    /// </summary>
    public class OssSignModel
    {
        public string policy { get; set; }
        public string accessid { get; set; }
        public string signature { get; set; }
        public string host { get; set; }
        public long expire { get; set; }
        public string dir { get; set; }
        /// <summary>
        /// 回调地址--BASE64编码Json字符串后
        /// </summary>
        public string callback { get; set; }
    }
}
