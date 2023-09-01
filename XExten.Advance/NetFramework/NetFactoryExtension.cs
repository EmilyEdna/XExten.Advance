using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XExten.Advance.IocFramework;
using XExten.Advance.NetFramework.Enums;

namespace XExten.Advance.NetFramework
{
    /// <summary>
    /// 网络请求
    /// </summary>
    public static class NetFactoryExtension
    {      
        static NetFactoryExtension()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);         
        }

        /// <summary>
        /// 注入限定类
        /// </summary>
        /// <param name="Lifespan">HttpClient生命周期 默认1秒钟</param>
        /// <param name="platform">使用平台</param>
        public static void RegisterNetFramework(int Lifespan = 1, Platform platform = Platform.Windows)
        {
            ConstDefault.Platform = platform;
            IocDependency._Services.AddHttpClient(string.Empty, opt => opt.DefaultRequestHeaders.UserAgent.ParseAdd(ConstDefault.GetPlatformAgentValue()))
                .SetHandlerLifetime(TimeSpan.FromSeconds(Lifespan));
            IocDependency.Register<INetFactory, RestFactory>(1);
            IocDependency.Register<INetFactory, HttpFactory>(1);
        }

        /// <summary>
        /// 限定获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>() where T : INetFactory
        {
            var Service = IocDependency.Resolves<T>();
            if (!Service.Any()) return default;
            var index = new Random().Next(2);
            if (index == 0) return Service.FirstOrDefault();
            else return Service.LastOrDefault();
        }
    }
}
