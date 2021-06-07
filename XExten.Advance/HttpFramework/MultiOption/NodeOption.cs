using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using XExten.Advance.HttpFramework.MultiCommon;
using XExten.Advance.LinqFramework;
using XExten.Advance.StaticFramework;

namespace XExten.Advance.HttpFramework.MultiOption
{
    /// <summary>
    /// NodeOption
    /// </summary>
    public class NodeOption
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string NodePath { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        public MultiType ReqType { get; set; } = MultiType.GET;
        /// <summary>
        /// 编码
        /// </summary>
        public string Encoding { get; set; } = "UTF-8";
        /// <summary>
        /// 缓存当前节点
        /// </summary>
        public bool CacheNode { get; set; } = false;
        /// <summary>
        /// Json格式参数
        /// </summary>
        public string JsonParam { get; set; }
        /// <summary>
        /// 实体参数
        /// </summary>
        public object EntityParam { get; set; }
        /// <summary>
        /// 表单参数
        /// </summary>
        public List<KeyValuePair<String, String>> FormParam { get; set; }
        /// <summary>
        /// 字段映射
        /// </summary>
        public IDictionary<string, string> MapFied { get; set; } = null;
        /// <summary>
        /// 请求内容
        /// </summary>
        internal HttpContent Contents { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        internal MediaTypeHeaderValue MediaTypeHeader { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        internal Uri URI { get; set; }
        internal void SetNode()
        {
            SyncStatic.TryCatch(() =>
            {
                if (NodePath.IsNullOrEmpty())
                    throw new Exception("请求地址不能为空!");
                URI = new Uri(NodePath);
                if (!JsonParam.IsNullOrEmpty() && ReqType != MultiType.DELETE && ReqType != MultiType.GET)
                {
                    Contents = new StringContent(JsonParam);
                    MediaTypeHeader = new MediaTypeHeaderValue("application/json");
                }
                if (!JsonParam.IsNullOrEmpty() && (ReqType == MultiType.DELETE || ReqType == MultiType.GET))
                    URI = new Uri(NodePath + JsonParam.ToModel<JObject>().ByUri());
                if (FormParam != null && FormParam != null && FormParam.Count > 0 && ReqType != MultiType.DELETE && ReqType != MultiType.GET)
                {
                    Contents = new FormUrlEncodedContent(FormParam);
                    MediaTypeHeader = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                }
                if (FormParam != null && FormParam != null && FormParam.Count > 0 && (ReqType == MultiType.DELETE || ReqType == MultiType.GET))
                    URI = new Uri(NodePath + FormParam.ByUri());
                if (EntityParam != null && ReqType != MultiType.DELETE && ReqType != MultiType.GET)
                {
                    Contents = new FormUrlEncodedContent(MultiKeyPairs.KeyValuePairs(EntityParam, MapFied));
                    MediaTypeHeader = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                }
                if (EntityParam != null && (ReqType == MultiType.DELETE || ReqType == MultiType.GET))
                    URI = new Uri(NodePath + EntityParam.ByUri());
                MultiConfig.NodeOpt.Add(this);
            }, ex => throw ex);
        }
    }
}
