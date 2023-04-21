using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using XExten.Advance.LinqFramework;
using XExten.Advance.NetFramework.Enums;
using XExten.Advance.StaticFramework;

namespace XExten.Advance.NetFramework.Options
{
    /// <summary>
    /// 节点
    /// </summary>
    public class DefaultNodes
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Encoding { get; set; } = "UTF-8";
        /// <summary>
        /// 请求地址
        /// </summary>
        public string Node { get; set; }
        /// <summary>
        /// 请求方法
        /// </summary>
        public Method Method { get; set; } = Method.GET;
        /// <summary>
        /// 请求类型
        /// </summary>
        public Category Category { get; set; } = Category.None;
        /// <summary>
        /// 参数
        /// </summary>
        public object Parameter { get; set; } = null;
        /// <summary>
        /// 字段映射
        /// </summary>
        public IDictionary<string, string> MapFied { get; set; } = null;
        /// <summary>
        /// 请求内容
        /// </summary>
        internal HttpContent Contents { get; set; }
        internal void SetNode()
        {
            SyncStatic.TryCatch(() =>
            {
                if (Node.IsNullOrEmpty())
                    throw new Exception("请求地址不能为空!");
                if (Category == Category.Json && (Method == Method.PUT || Method == Method.POST))
                {
                    Contents = new StringContent(Parameter == null ? "" : ((Parameter is string) ? Parameter.ToString() : Parameter.ToJson()));
                    Contents.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }
                if (Category == Category.Form && (Method == Method.PUT || Method == Method.POST))
                {
                    if (Parameter is List<KeyValuePair<String, String>> Target)
                        Contents = new FormUrlEncodedContent(Target);
                    else Contents = new FormUrlEncodedContent(MultiKeyPairs.KeyValuePairs(Parameter, MapFied));
                    Contents.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                }
            }, ex => throw ex);
        }
    }
}
