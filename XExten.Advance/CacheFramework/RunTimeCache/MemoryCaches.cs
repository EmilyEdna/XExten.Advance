using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;


namespace XExten.Advance.CacheFramework.RunTimeCache
{
    /// <summary>
    ///
    /// </summary>
    public class MemoryCaches
    {
        private static readonly object locker = new object();
        private static IMemoryCache _Cache;
        /// <summary>
        /// MemoryCaches
        /// </summary>
        public static IMemoryCache Cache
        {
            get
            {
                if (_Cache == null) 
                {
                    lock (locker)
                    {
                        if (_Cache == null)
                            _Cache = new MemoryCache(new MemoryCacheOptions());
                    }
                }
                return _Cache;
            }
        }

        /// <summary>
        /// 获取所有Key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetKeys<T>()
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var Field = Cache.GetType().GetField("_coherentState", flags).GetValue(Cache);
            var Keys = (Field.GetType().GetProperty("EntriesCollection", flags).GetValue(Field) as IDictionary).Keys.OfType<T>().ToList();
            return Keys;
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <param name="Time"></param>
        /// <param name="UseSecond"></param>
        public static void AddCache<T>(String Key, T Value, int Time, bool UseSecond = false)
        {
            Cache.Set(Key, Value, new DateTimeOffset((UseSecond ? DateTime.Now.AddSeconds(Time) : DateTime.Now.AddMinutes(Time))));
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static T GetCache<T>(String Key)
        {
            return Cache.Get(Key) == null ? default(T) : (T)Cache.Get(Key);
        }

        /// <summary>
        /// 删除指定缓存
        /// </summary>
        /// <param name="Key"></param>
        public static void RemoveCache(String Key)
        {
            Cache.Remove(Key);
        }

        /// <summary>
        /// 删除所有缓存
        /// </summary>
        public static void RemoveAllCache() 
        {
            var AllKey = GetKeys<string>();
            foreach (var item in AllKey)
            {
                RemoveCache(item);
            }
        }
    }
}