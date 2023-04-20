using Example;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using XExten.Advance.LinqFramework;

namespace XExten.Advance.NetFramework
{
    /// <summary>
    /// 网络请求
    /// </summary>
    public static class NetFactoryExtension
    {
        private static IServiceCollection _Services;
        private static IServiceProvider _Provider;
        static NetFactoryExtension()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _Services ??= new ServiceCollection();
        }

        /// <summary>
        /// 限定获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>() where T : INetFactory
        {
            if (_Provider == null) return default;
            if (!_Provider.GetServices<T>().Any()) return default;
            var index = new Random().Next(2);
            if (index == 0) return _Provider.GetServices<T>().FirstOrDefault();
            else return _Provider.GetServices<T>().LastOrDefault();
        }
        /// <summary>
        /// 通用获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            if (_Provider == null) return default;
            return _Provider.GetService<T>();
        }
        /// <summary>
        /// 注入限定类
        /// </summary>
        public static void RegisterNetFramework()
        {
            var head = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36 Edg/112.0.1722.48";
            _Services.AddHttpClient(string.Empty, opt => opt.DefaultRequestHeaders.UserAgent.ParseAdd(head))
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));
            _Services.AddTransient<INetFactory, RestFactory>();
            _Services.AddTransient<INetFactory, HttpFactory>();
            _Provider = _Services.BuildServiceProvider();
        }
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="action"></param>
        public static void RegisterService(Action<IServiceCollection> action)
        {
            action.Invoke(_Services);
            _Provider = _Services.BuildServiceProvider();
        }
    }
}
