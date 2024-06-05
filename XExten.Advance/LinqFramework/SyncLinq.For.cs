using AutoMapper;
using Mapster;
using MessagePack;
using MessagePack.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using XExten.Advance.InternalFramework.Express;
using XExten.Advance.InternalFramework.Securities;

namespace XExten.Advance.LinqFramework
{
    /// <summary>
    /// LinqExtension
    /// </summary>
    public static partial class SyncLinq
    {
        #region  For
        /// <summary>
        /// 遍历字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public static void ForDicEach<T, K>(this IDictionary<T, K> param, Action<T, K> action)
        {
            foreach (KeyValuePair<T, K> item in param)
                action(item.Key, item.Value);
        }

        /// <summary>
        /// 遍历字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public static void ForDicEach<T, K>(this IDictionary<T, K> param, Action<T, K,int> action)
        {
            for (int index = 0; index < param.Count(); index++)
                action(param.Keys.ElementAtOrDefault(index), param.Values.ElementAtOrDefault(index), index);
        }

        /// <summary>
        /// 遍历数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public static void ForArrayEach<T>(this Array param, Action<T> action)
        {
            foreach (var item in param)
                action((T)item);
        }

        /// <summary>
        /// 遍历数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public static void ForArrayEach<T>(this Array param, Action<T,int> action)
        {
            for (int index = 0; index < param.Length; index++)
                action((T)param.GetValue(index), index);
        }

        /// <summary>
        /// 遍历集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public static void ForEnumerEach<T>(this IEnumerable<T> param, Action<T> action)
        {
            foreach (var item in param)
                action(item);
        }

        /// <summary>
        /// 遍历集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public static void ForEnumerEach<T>(this IEnumerable<T> param, Action<T, int> action)
        {
            for (int index = 0; index < param.Count(); index++)
                action(param.ElementAtOrDefault(index), index);
        }
        #endregion

        #region Async
        /// <summary>
        /// 遍历字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="param"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task ForDicEachAsync<T, K>(this IDictionary<T, K> param, Action<T, K> action) => await Task.Run(() => ForDicEach(param, action));

        /// <summary>
        /// 遍历字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public static async Task ForDicEachAsync<T, K>(this IDictionary<T, K> param, Action<T, K, int> action) => await Task.Run(() => ForDicEach(param, action));

        /// <summary>
        /// 遍历数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public static async Task ForArrayEachAsync<T>(this Array param, Action<T> action) => await Task.Run(() => ForArrayEach(param, action));

        /// <summary>
        /// 遍历数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public static async Task ForArrayEachAsync<T>(this Array param, Action<T, int> action) => await Task.Run(() => ForArrayEach(param, action));

        /// <summary>
        /// 遍历集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public static async Task ForEnumerEachAsync<T>(this IEnumerable<T> param, Action<T> action) => await Task.Run(() => ForEnumerEach(param, action));

        /// <summary>
        /// 遍历集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public static async Task ForEnumerEachAsync<T>(this IEnumerable<T> param, Action<T, int> action) => await Task.Run(() => ForEnumerEach(param, action));
        #endregion
    }
}
