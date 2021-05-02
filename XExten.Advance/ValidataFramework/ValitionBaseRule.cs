using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Advance.ValidataFramework
{
    /// <summary>
    /// 校验规则
    /// </summary>
    public abstract class ValitionBaseRule
    {
        /// <summary>
        /// 校验逻辑
        /// </summary>
        /// <param name="pairs"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public abstract  Task<(bool Success, object Data)> ValitionRules(Dictionary<string,string> pairs,params ParameterInfo[] parameters);
    }
}
