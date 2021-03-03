using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Synctool.ValidataFramework.Enum;
using Synctool.Linq;
using System.Text.RegularExpressions;

namespace Synctool.ValidataFramework
{
    /// <summary>
    /// 校验规则
    /// </summary>
    public class ValitionRule : ValitionBaseRule
    {
        /// <summary>
        /// 验证参数
        /// </summary>
        /// <param name="pairs"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override async Task<(bool Success, object Data)> ValitionRules(Dictionary<string, string> pairs, params ParameterInfo[] parameters)
        {
            foreach (var Item in parameters)
            {
                if (!Item.ParameterType.Namespace.Contains("System"))
                {
                    ValitionClassAttribute Class = Item.ParameterType.GetCustomAttribute(typeof(ValitionClassAttribute)) as ValitionClassAttribute;
                    if(Class==null) return await Task.FromResult((true, ""));
                    Dictionary<string, ValitionAttribute> KeyValues = Item.ParameterType.GetProperties()
                         .Where(t => t.GetCustomAttribute(typeof(ValitionAttribute)) != null)
                         .ToDictionary(t => t.Name, t => t.GetCustomAttributes(typeof(ValitionAttribute),true).FirstOrDefault() as ValitionAttribute);
                    if (Class.ModuleType == ValitionModuleEnum.Single)
                    {
                        foreach (var item in KeyValues)
                        {
                            var key = item.Key;
                            var val = item.Value;
                            if (val.CustomerValition != null && val.ValiType == ValitionEnum.Customer)
                            {
                                pairs.TryGetValue(key, out string result);
                                val.InfoMsg = val.CustomerValition.Invoke(result);
                                if (val.UsageAppendField && val.InfoMsg.IsNullOrEmpty())
                                    val.InfoMsg = key + ":" + val.InfoMsg;
                                return await Task.FromResult((false, val.InfoMsg));
                            }
                            else if (val.ValiType == ValitionEnum.Regex)
                            {
                                Regex reg = new Regex(val.RegexStr, RegexOptions.Compiled);
                                pairs.TryGetValue(key, out string result);
                                if (!reg.IsMatch(result.ToString()))
                                    return await Task.FromResult((false, val.UsageAppendField ? val.InfoMsg = key + ":" + val.InfoMsg : val.InfoMsg));
                            }
                            else
                            {
                                pairs.TryGetValue(key, out string result);
                                if (result.IsNullOrEmpty())
                                    return await Task.FromResult((false, val.UsageAppendField ? val.InfoMsg = key + ":" + val.InfoMsg : val.InfoMsg));
                            }
                        }
                    }
                    else {
                        List<string> results = new List<string>();
                        foreach (var item in KeyValues)
                        {
                            var key = item.Key;
                            var val = item.Value;
                            if (val.CustomerValition != null && val.ValiType == ValitionEnum.Customer)
                            {
                                pairs.TryGetValue(key, out string result);
                                val.InfoMsg = val.CustomerValition.Invoke(result);
                                if (val.UsageAppendField && val.InfoMsg.IsNullOrEmpty())
                                    val.InfoMsg = key + ":" + val.InfoMsg;
                                results.Add(val.InfoMsg);
                            }
                            else if (val.ValiType == ValitionEnum.Regex)
                            {
                                Regex reg = new Regex(val.RegexStr, RegexOptions.Compiled);
                                pairs.TryGetValue(key, out string result);
                                if (!reg.IsMatch(result.ToString()))
                                    results.Add(val.UsageAppendField ? val.InfoMsg = key + ":" + val.InfoMsg : val.InfoMsg);
                            }
                            else
                            {
                                pairs.TryGetValue(key, out string result);
                                if (result.IsNullOrEmpty())
                                    results.Add(val.UsageAppendField ? val.InfoMsg = key + ":" + val.InfoMsg : val.InfoMsg);
                            }
                        }
                        if(results.Count>0)
                            return await Task.FromResult((false, results));
                    }
                }
            }
            return await Task.FromResult((true, ""));
        }
    }
}
