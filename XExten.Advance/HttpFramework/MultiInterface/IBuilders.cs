using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.HttpFramework.MultiOption;

namespace XExten.Advance.HttpFramework.MultiInterface
{
    /// <summary>
    /// builder
    /// </summary>
    public interface IBuilders
    {
        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="action"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        IBuilders Build(Action<BuilderOption> action = null, Action<HttpClientHandler> handle = null);
        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="Container"></param>
        /// <returns></returns>
        List<Byte[]> RunBytes(Action<CookieContainer, Uri> Container = null);
        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="Container"></param>
        /// <returns></returns>
        Task<List<Byte[]>> RunBytesAsync(Action<CookieContainer, Uri> Container = null);
        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="Container"></param>
        /// <returns></returns>
        List<string> RunString(Action<CookieContainer, Uri> Container = null);
        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="Container"></param>
        /// <returns></returns>
        Task<List<string>> RunStringAsync(Action<CookieContainer, Uri> Container = null);
    }
}
