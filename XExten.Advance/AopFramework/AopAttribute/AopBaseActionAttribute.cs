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
        /// <summary>
        /// 自定义执行
        /// </summary>
        public string ActionType { get; set; }
        /// <summary>
        /// 执行器无参构造
        /// </summary>
        public AopBaseActionAttribute()
        {

        }
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="ActionType"></param>
        public AopBaseActionAttribute(string ActionType)
        {
            this.ActionType = ActionType;
        }
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="classInfo"></param>
        /// <param name="parameters"></param>
        public virtual void Before(string methodName, Type classInfo, object[] parameters) { }
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="classInfo"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual object After(string methodName, Type classInfo, object result) => result;
    }
}
