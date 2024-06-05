using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XExten.Advance.LinqFramework
{
    public static partial class SyncLinq
    {

        #region With
        /// <summary>
        /// 返回实体中所有的字段名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<string> WithNames<T>(this IEnumerable<T> param) where T : class, new()
        {
            List<String> Names = new List<String>();
            param.FirstOrDefault().GetType().GetProperties().ForEnumerEach(t =>
            {
                Names.Add(t.Name);
            });
            return Names;
        }

        /// <summary>
        /// 返回一个实体中所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Dictionary<string, object> WithKeyValue<T>(T param) where T : class, new()
        {
            var result = new Dictionary<string, object>();

            param.GetType().GetProperties().ForEnumerEach(item =>
            {
                result.Add(item.Name, item.GetValue(param));
            });

            return result;
        }
        #endregion

        #region Async
        /// <summary>
        /// 返回实体中所有的字段名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<List<string>> WithNamesAsync<T>(this IEnumerable<T> param) where T : class, new() => await Task.Run(() => WithNames(param));
        
        /// <summary>
        /// 返回一个实体中所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, object>> WithKeyValueAsync<T>(T param) where T : class, new() => await Task.Run(() => WithKeyValue(param));
        #endregion
    }
}
