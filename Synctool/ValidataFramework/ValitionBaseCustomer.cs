using System;
using System.Collections.Generic;
using System.Text;

namespace Synctool.ValidataFramework
{
    /// <summary>
    /// 自定义验证父类
    /// </summary>
    public abstract class ValitionBaseCustomer
    {
        /// <summary>
        /// 自定义验证
        /// </summary>
        /// <param name="requestParam">请求参数</param>
        /// <returns>T1 是否符合要求,T2 验证信息</returns>
        public abstract (bool Success,string Info) UserCustomerValition(string requestParam);
    }
}
