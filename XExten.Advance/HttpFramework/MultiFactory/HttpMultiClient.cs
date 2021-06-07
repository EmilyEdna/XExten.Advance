using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using XExten.Advance.HttpFramework.MultiCommon;
using XExten.Advance.HttpFramework.MultiImplement;
using XExten.Advance.HttpFramework.MultiInterface;
using XExten.Advance.HttpFramework.MultiOption;
using XExten.Advance.LinqFramework;

namespace XExten.Advance.HttpFramework.MultiFactory
{
    internal class HttpMultiClient : IHttpMultiClient
    {
        public HttpMultiClient()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            MultiConfig.Builder = new Builders();
            MultiConfig.Headers = new Headers();
            MultiConfig.Cookies = new Cookies();
            MultiConfig.Nodes = new Nodes();
        }
        public IHttpMultiClient InitWebProxy(Action<MultiProxy> action)
        {
            MultiProxy Proxy = new MultiProxy();
            action(Proxy);
            if (Proxy.IP.IsNullOrEmpty() || Proxy.Port == -1)
                return this;
            MultiConfig.Proxy = new WebProxy(Proxy.IP, Proxy.Port);
            if (!Proxy.UserName.IsNullOrEmpty() && !Proxy.PassWord.IsNullOrEmpty())
                MultiConfig.Proxy.Credentials = new NetworkCredential(Proxy.UserName, Proxy.PassWord);
            return this;
        }

        public IHttpMultiClient InitWebProxy(MultiProxy proxy)
        {
            if (proxy.IP.IsNullOrEmpty() || proxy.Port == -1)
                return this;
            MultiConfig.Proxy = new WebProxy(proxy.IP, proxy.Port);
            if (!proxy.UserName.IsNullOrEmpty() && !proxy.PassWord.IsNullOrEmpty())
                MultiConfig.Proxy.Credentials = new NetworkCredential(proxy.UserName, proxy.PassWord);
            return this;
        }

        public IHttpMultiClient InitCookie()
        {
            if (MultiConfig.Container == null)
                MultiConfig.Container = new CookieContainer();
            return this;
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
            HeaderOption Option = new HeaderOption();
            action(Option);
            Option.SetHeader();
            return MultiConfig.Headers;
        }

        public INodes AddNode(Action<NodeOption> action)
        {
            NodeOption Option = new NodeOption();
            action(Option);
            Option.SetNode();
            return MultiConfig.Nodes;
        }
    }
}
