using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using XExten.Advance.HttpFramework.MultiCommon;
using XExten.Advance.HttpFramework.MultiInterface;
using XExten.Advance.HttpFramework.MultiOption;

namespace XExten.Advance.HttpFramework.MultiImplement
{
    internal class Cookies : ICookies
    {
        public IBuilders Build(Action<BuilderOption> action = null, Action<HttpClientHandler> handle = null)
        {
            return MultiConfig.Builder.Build(action, handle);
        }

        public ICookies AddCookie(Action<CookieOption> action)
        {
            CookieOption Option = new CookieOption();
            action(Option);
            Option.SetCookie();
            return MultiConfig.Cookies;
        }

        public IHeaders AddHeader(Action<HeaderOption> action)
        {
            return MultiConfig.Headers.AddHeader(action);
        }

        public INodes AddNode(Action<NodeOption> action)
        {
            return MultiConfig.Nodes.AddNode(action);
        }
    }
}
