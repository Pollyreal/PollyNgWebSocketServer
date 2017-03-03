using Aliyun.OSS;
using Jil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PollyNg.API.Service
{
    /// <summary>
    /// 阿里云对象存储 OSS
    /// </summary>
    public class OssHelper
    {
        private string _endpoint = "oss-cn-shanghai.aliyuncs.com";
        private string _bucket = "pollyng";
        private string _dir = "pollyng/";

        /// <summary>
        /// 用户请求的accessid
        /// </summary>
        private string _accessId = BaseConfig.OssAccessId;
        private string _accessKey = BaseConfig.OssAccessKey;
        private long _expireTime = 10;
        private string _callbackUrl = "";
        private CallBackParam callbackparam = new CallBackParam();
        private DateTime BaseTime = new DateTime(1970, 1, 1);//Unix起始时间
        public string endpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }
        public string bucket
        {
            get { return _bucket; }
            set { _bucket = value; }
        }
        /// <summary>
        /// 上传目录pollyng/(必须以‘/’结尾)
        /// </summary>
        public string dir
        {
            get { return _dir; }
            set { _dir = value; }
        }
        public string accessId
        {
            get { return _accessId; }
            set { _accessId = value; }
        }
        public string accessKey
        {
            get { return _accessKey; }
            set { _accessKey = value; }
        }
        /// <summary>
        /// 默认10秒过期
        /// </summary>
        public long expireTime
        {
            get { return _expireTime; }
            set { _expireTime = value; }
        }
        /// <summary>
        /// 默认回调地址
        /// </summary>
        public string callbackUrl
        {
            get { return _callbackUrl; }
            set { _callbackUrl = value; }
        }
        /// <summary>
        /// 默认上传目录pollyng/;传图回调页面http://demo.duokongkeji.com
        /// </summary>
        public OssHelper()
        {
        }

        /// <summary>
        /// 配置上传目录(必须以‘/’结尾),传图回调页面；
        /// </summary>
        /// <param name="directory">上传目录</param>
        /// <param name="callbackUrl">回调目录</param>
        public OssHelper(string directory = "", string callbackUrl = "")
        {
            if (directory != "")
            {
                this.dir = directory;
            }
            if (callbackUrl != "")
            {
                this.callbackUrl = callbackUrl;
            }
        }

        /// <summary>
        /// 获取oss凭证
        /// </summary>
        /// <returns></returns>
        public OssSignModel GetOssSign()
        {
            callbackparam.callbackUrl = callbackUrl;
            var host = string.Format("http://{0}.{1}", bucket, endpoint);
            OssClient client = new OssClient(endpoint, accessId, accessKey);
            DateTime expiration = DateTime.Now.AddSeconds(expireTime);
            PolicyConditions policyConds = new PolicyConditions();
            policyConds.AddConditionItem(PolicyConditions.CondContentLengthRange, 0, 1050289624);
            policyConds.AddConditionItem(MatchMode.StartWith, PolicyConditions.CondKey, dir);

            string postPolicy = client.GeneratePostPolicy(expiration.AddHours(8), policyConds);
            string encodedPolicy = Convert.ToBase64String(Encoding.UTF8.GetBytes(postPolicy.ToCharArray()));
            string postSignature = HmacSHA1Signature(accessKey, encodedPolicy);

            var ossmodel = new OssSignModel();
            ossmodel.dir = dir;
            ossmodel.host = host;
            ossmodel.accessid = accessId;
            ossmodel.policy = encodedPolicy;
            ossmodel.signature = postSignature;
            ossmodel.expire = (expiration.Ticks - BaseTime.Ticks) / 10000000 - 8 * 60 * 60;
            ossmodel.callback = Convert.ToBase64String(Encoding.UTF8.GetBytes(JSON.Serialize(callbackparam).ToCharArray()));
            return ossmodel;
        }

        /// <summary>
        /// HmacSHA1签名
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string HmacSHA1Signature(string key, string data)
        {
            string SignatureMethod = "HmacSHA1";
            using (var algorithm = KeyedHashAlgorithm.Create(SignatureMethod.ToUpperInvariant()))
            {
                algorithm.Key = Encoding.UTF8.GetBytes(key.ToCharArray());
                return Convert.ToBase64String(
                    algorithm.ComputeHash(Encoding.UTF8.GetBytes(data.ToCharArray())));
            }
        }

    }

}