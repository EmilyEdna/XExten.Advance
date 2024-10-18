using XExten.Advance.ValidataFramework.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Advance.ValidataFramework
{
    /// <summary>
    /// 参数验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ValitionClassAttribute: Attribute
    {
        /// <summary>
        /// 验证模式
        /// </summary>
        public ValitionModuleEnum ModuleType { get; set; } = ValitionModuleEnum.Single;
    }
}
