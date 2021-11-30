using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Advance.AopFramework.AopAttribute
{
    /// <summary>
    /// 执行器
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AopBaseActionAttribute : Attribute
    {

        public string Code { get; set; }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="classInfo"></param>
        /// <param name="parameters"></param>
        public virtual void Before(string methodName,string classInfo, object[] parameters) { }
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="classInfo"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual object After(string methodName, string classInfo, object result) => result;
    }
}
