using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using XExten.Advance.NetFramework.Options;

namespace XExten.Advance.NetFramework
{
    /// <summary>
    /// 网络请求工厂
    /// </summary>
    public interface INetFactory
    {
        /// <summary>
        /// 添加代理
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="Port"></param>
        /// <returns></returns>
        INetFactory AddProxy(string IP, int Port);
        /// <summary>
        ///  添加代理
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="IP"></param>
        /// <param name="Port"></param>
        /// <returns></returns>
        INetFactory AddWhreProxy(bool condition, string IP, int Port);
        /// <summary>
        /// 添加请求地址
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        INetFactory AddNode(Action<DefaultNodes> action);
        /// <summary>
        /// 添加请求地址
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        INetFactory AddNode(List<DefaultNodes> action);
        /// <summary>
        /// 添加请求头
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        INetFactory AddHeader(Action<DefaultHeader> action);
        /// <summary>
        /// 添加请求头
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        INetFactory AddHeader(List<DefaultHeader> action);
        /// <summary>
        /// 添加请求头
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        INetFactory AddWhereHeader(bool condition, List<DefaultHeader> action);
        /// <summary>
        /// 添加请求头
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        INetFactory AddWhereHeader(bool condition, Action<DefaultHeader> action);
        /// <summary>
        /// 添加认证
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        INetFactory AddCookie(Action<DefaultCookie> action);
        /// <summary>
        /// 设置顶级Uri
        /// </summary>
        /// <param name="Uri"></param>
        /// <returns></returns>
        INetFactory SetBaseUri(string Uri);
        /// <summary>
        /// 构建请求
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        INetFactory Build(Action<DefaultBuilder> action = null);
        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        INetFactory GetCookie(Action<CookieContainer, Uri> action);
        /// <summary>
        /// 返回Bytes
        /// </summary>
        /// <returns></returns>
        Task<List<byte[]>> RunBytes();
        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <returns></returns>
        Task<List<string>> RunString();
        /// <summary>
        /// 返回流
        /// </summary>
        /// <returns></returns>
        Task<List<Stream>> RunStream();

    }
}
