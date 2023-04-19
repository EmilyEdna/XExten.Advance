using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Advance.RestHttpFramework.Options
{
    /// <summary>
    /// 节点
    /// </summary>
    public class RestNode
    {
        /// <summary>
        /// 请求地址
        /// </summary>
        public string Route { get; set; }
        /// <summary>
        /// 是否启用缓存默认启用
        /// </summary>
        public bool UseCache { get; set; } = true;
        /// <summary>
        /// 请求方式
        /// <see cref="RestProviderMethod"/>
        /// </summary>
        public RestProviderMethod Provider { get; set; } = RestProviderMethod.GET;
        /// <summary>
        /// 方式
        /// </summary>
        public RestProviderType ProviderType { get; set; } = RestProviderType.JSON;
        /// <summary>
        /// 参数名称
        /// </summary>
        public List<string> PropertyNames { get; set; } = new List<string>();
        /// <summary>
        /// 编码
        /// </summary>
        public string Encoding { get; set; } = "UTF-8";
        /// <summary>
        /// 缓存时常默认5分钟
        /// </summary>
        public int CacheSpan { get; set; } = 5;
        /// <summary>
        /// 参数
        /// </summary>
        public object Param { get; set; }
        internal void SetNode(RestNode input)
        {
            OptionBuilder.Nodes.Add(input);
        }
    }
}
