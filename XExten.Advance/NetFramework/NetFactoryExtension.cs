using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text;
using XExten.Advance.NetFramework.Enums;

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

        #region 注册限定
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
        /// 注入限定类
        /// </summary>
        /// <param name="Lifespan">HttpClient生命周期 默认1秒钟</param>
        /// <param name="platform">使用平台</param>
        public static void RegisterNetFramework(int Lifespan=1, Platform platform= Platform.Windows)
        {
            ConstDefault.Platform = platform;
            _Services.AddHttpClient(string.Empty, opt => opt.DefaultRequestHeaders.UserAgent.ParseAdd(ConstDefault.GetPlatformAgentValue()))
                .SetHandlerLifetime(TimeSpan.FromSeconds(Lifespan));
            _Services.AddTransient<INetFactory, RestFactory>();
            _Services.AddTransient<INetFactory, HttpFactory>();
            _Provider = _Services.BuildServiceProvider();
        }
        #endregion

        #region 注册通用
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
        /// 注册服务
        /// </summary>
        /// <param name="action"></param>
        public static void RegisterService(Action<IServiceCollection> action)
        {
            action.Invoke(_Services);
            _Provider = _Services.BuildServiceProvider();
        }
        #endregion
    }
}
