using System;
using System.Collections.Generic;
using System.Text;

namespace Synctool.ValidataFramework.Enum
{
    /// <summary>
    /// 验证模式
    /// </summary>
    public enum ValitionModuleEnum
    {
        /// <summary>
        /// 单个验证不满足则返回
        /// </summary>
        Single,
        /// <summary>
        /// 全局验证完统一返回
        /// </summary>
        Global
    }
}
