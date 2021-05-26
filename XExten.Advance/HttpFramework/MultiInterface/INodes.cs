using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using XExten.Advance.HttpFramework.MultiOption;

namespace XExten.Advance.HttpFramework.MultiInterface
{
    /// <summary>
    /// Node
    /// </summary>
    public interface INodes
    {
        /// <summary>
        /// Builder
        /// </summary>
        /// <param name="action"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        IBuilders Build(Action<BuilderOption> action = null, Action<HttpClientHandler> handle = null);
        /// <summary>
        /// Cookie
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ICookies AddCookie(Action<CookieOption> action);
        /// <summary>
        /// Node
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        INodes AddNode(Action<NodeOption> action);
        /// <summary>
        /// Header
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IHeaders AddHeader(Action<HeaderOption> action);
    }
}
