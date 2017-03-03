using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PollyNg.API
{
    public class BaseConfig
    {
        /// <summary>
        /// Redis配置
        /// </summary>
        public static string RedisPath = System.Configuration.ConfigurationManager.AppSettings["Cache_Redis_Configuration"];
        public static string RedisKey = System.Configuration.ConfigurationManager.AppSettings["Cache_Redis_Key"];
        public static string SocketAddress = System.Configuration.ConfigurationManager.AppSettings["Socket_Address"];
        //OSS配置
        public static string OssAccessId = System.Configuration.ConfigurationManager.AppSettings["Oss_AccessId"];
        public static string OssAccessKey = System.Configuration.ConfigurationManager.AppSettings["Oss_AccessKey"];
    }
}