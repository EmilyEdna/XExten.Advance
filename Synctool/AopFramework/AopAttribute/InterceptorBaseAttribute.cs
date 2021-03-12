using System;
using System.Collections.Generic;
using System.Text;

namespace Synctool.AopFramework.AopAttribute
{
    /// <summary>
    /// 拦截器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class InterceptorBaseAttribute : Attribute
    {
        /// <summary>
        /// 执行器
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual object Invoke(object obj, string methodName, object[] parameters)
        {
            return obj.GetType().GetMethod(methodName).Invoke(obj, parameters);
        }
    }
}
