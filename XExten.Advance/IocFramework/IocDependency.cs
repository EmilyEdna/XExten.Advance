using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace XExten.Advance.IocFramework
{
    /// <summary>
    /// Ioc
    /// </summary>
    public class IocDependency
    {
        internal static IServiceCollection _Services;
        internal static IServiceProvider _Provider;
        static IocDependency()
        {
            _Services ??= new ServiceCollection();
        }

        /// <summary>
        ///  注册
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="Module">1 瞬时 2 单例 3 作用域</param>
        public static void Register<TService, TImplementation>(int Module=2) where TService : class where TImplementation : class, TService
        {
            if(Module==1)
                _Services.AddTransient<TService, TImplementation>();
            else if (Module==2)
                _Services.AddSingleton<TService, TImplementation>();
            else
                _Services.AddScoped<TService, TImplementation>();

            _Provider = _Services.BuildServiceProvider();
        }

        /// <summary>
        ///  注册
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="Module">1 瞬时 2 单例 3 作用域</param>
        public static void Register(Type serviceType, Type implementationType, int Module = 2) 
        {
            if (Module == 1)
                _Services.AddTransient(serviceType, implementationType);
            else if (Module == 2)
                _Services.AddSingleton(serviceType, implementationType);
            else
                _Services.AddScoped(serviceType, implementationType);

            _Provider = _Services.BuildServiceProvider();
        }

        /// <summary>
        ///  注册
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="Module">1 瞬时 2 单例 3 作用域</param>
        public static void Register(Type serviceType, int Module = 2)
        {
            if (Module == 1)
                _Services.AddTransient(serviceType);
            else if (Module == 2)
                _Services.AddSingleton(serviceType);
            else
                _Services.AddScoped(serviceType);

            _Provider = _Services.BuildServiceProvider();
        }

        /// <summary>
        /// 取出实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>() 
        {
            if (_Provider == null) return default;
            return _Provider.GetService<T>();
        }

        /// <summary>
        /// 取出实例集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Resolves<T>()
        {
            if (_Provider == null) return default;
            return _Provider.GetServices<T>();
        }

        /// <summary>
        /// 取出实例
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static object Resolve(Type serviceType) 
        {
            if (_Provider == null) return default;
            return _Provider.GetService(serviceType);
        }

        /// <summary>
        /// 取出实例集合
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static object Resolves(Type serviceType)
        {
            if (_Provider == null) return default;
            return _Provider.GetServices(serviceType);
        }
    }
}
