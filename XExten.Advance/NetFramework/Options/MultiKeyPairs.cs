using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using XExten.Advance.LinqFramework;

namespace XExten.Advance.NetFramework.Options
{
    /// <summary>
    /// 封装数据
    /// </summary>
    public class MultiKeyPairs
    {
        /// <summary>
        /// 创建一个key-value格式的表单数据(Making form data to KeyValuePairs)
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Map"></param>
        /// <returns></returns>
        public static List<KeyValuePair<String, String>> KeyValuePairs(object Entity, IDictionary<string, string> Map = null)
        {
            return KeyValuePairs(Entity.GetType().GetProperties().ToList(), Entity, Map);
        }

        /// <summary>
        /// 创建一个key-value格式的表单数据(Making form data to KeyValuePairs)
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Map"></param>
        /// <returns></returns>
        public static List<KeyValuePair<String, String>> KeyValuePairs<T>(T Entity, IDictionary<string, string> Map = null) where T : class, new()
        {
            return KeyValuePairs(Entity.GetType().GetProperties().ToList(), Entity, Map);
        }

        private static List<KeyValuePair<String, String>> KeyValuePairs<T>(List<PropertyInfo> PropertyInfos, T Entity, IDictionary<string, string> Map = null)
        {
            List<KeyValuePair<String, String>> keyValuePairs = new List<KeyValuePair<string, string>>();
            PropertyInfos.ForEach(t =>
            {
                var flag = t.CustomAttributes.Where(x => x.AttributeType == typeof(JsonIgnoreAttribute)).FirstOrDefault();
                if (Map != null)
                {
                    if (!Map.Keys.Where(Item => Item.Equals(t.Name)).FirstOrDefault().IsNullOrEmpty())
                        keyValuePairs.Add(new KeyValuePair<string, string>(Map[t.Name], RetrunEnumValue(t, Entity)));
                    else
                    {
                        keyValuePairs.Add(new KeyValuePair<string, string>(t.Name, RetrunEnumValue(t, Entity)));
                    }
                }
                else if (flag == null)
                    keyValuePairs.Add(new KeyValuePair<string, string>(t.Name, RetrunEnumValue(t, Entity)));
            });
            return keyValuePairs;
        }

        /// <summary>
        /// 返回枚举值
        /// </summary>
        /// <returns></returns>
        private static string RetrunEnumValue<T>(PropertyInfo Info, T Entity)
        {
            if (Info.PropertyType.BaseType == typeof(Enum))
                return ((int)Enum.Parse(Info.PropertyType, Info.GetValue(Entity).ToString())).ToString();
            else
                return Info.GetValue(Entity).ToString();
        }
    }
}
