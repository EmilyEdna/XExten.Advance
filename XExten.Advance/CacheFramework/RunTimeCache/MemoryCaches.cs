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

        private static readonly Func<MemoryCache, object> GetEntriesCollection =
            Delegate.CreateDelegate(typeof(Func<MemoryCache, object>),
                typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true), true) as Func<MemoryCache, object>;

        /// <summary>
        /// 获取所有Key
        /// </summary>
        /// <returns></returns>
        public static IEnumerable GetKeys() => ((IDictionary)GetEntriesCollection((MemoryCache)Cache)).Keys;

        /// <summary>
        /// 获取所有Key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetKeys<T>() => GetKeys().OfType<T>().ToList();

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