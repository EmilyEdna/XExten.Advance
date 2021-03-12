using System;
using System.Collections.Generic;
using System.Text;

namespace Synctool.AopFramework.AopAttribute
{
    /// <summary>
    /// 执行器
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AopBaseActionAttribute : Attribute
    {
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        public virtual void Before(string methodName, object[] parameters) { }
        /// <summary>
        /// 执行后
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual object After(string methodName, object result) => result;
    }
}
