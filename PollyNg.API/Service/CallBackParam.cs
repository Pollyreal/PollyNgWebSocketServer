using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PollyNg.API.Service
{
    /// <summary>
    /// 回调地址参数
    /// </summary>
    public class CallBackParam
    {
        private string _callbackUrl = "";
        private string _callbackBody = "filename=${object}&size=${size}&mimeType=${mimeType}&height=${imageInfo.height}&width=${imageInfo.width}";
        private string _callbackBodyType = "application/x-www-form-urlencoded";
        public string callbackUrl { get { return _callbackUrl; } set { _callbackUrl = value; } }
        /// <summary>
        /// 上传回调基本信息
        ///系统变量	含义
        ///bucket	移动应用上传到哪个存储空间
        ///object	移动应用上传到OSS保存的文件名
        ///etag	该上传的文件的etag，即返回给用户的etag字段
        ///size	该上传的文件的大小
        ///mimeType	资源类型
        ///imageInfo.height	图片高度
        ///imageInfo.width	图片宽度
        ///imageInfo.format	图片格式，如jpg、png，只以识别图片
        /// </summary>
        public string callbackBody { get { return _callbackBody; } set { _callbackBody = value; } }
        public string callbackBodyType { get { return _callbackBodyType; } set { _callbackBodyType = value; } }
    }
}
