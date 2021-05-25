using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Advance.HttpFramework.MultiInterface
{
    /// <summary>
    /// 自定义解析器
    /// </summary>
    public interface IResolver
    {
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="Host"></param>
        /// <returns></returns>
        string Resolve(string Host);
    }
}
