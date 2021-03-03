using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Synctool.ValidataFramework
{
    /// <summary>
    /// 注册验证框架
    /// </summary>
    public static class ValitionExtension
    {
        /// <summary>
        /// 使用验证
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseValition(this IApplicationBuilder application)
        {
            return application.UseMiddleware<ValitionMiddleWare>();
        }
    }
}
