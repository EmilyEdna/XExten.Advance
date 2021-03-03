using System;
using System.Collections;
using Microsoft.Extensions.Caching.Memory;


namespace Synctool.CacheFactory.RunTimeCache
{
    /// <summary>
    ///
    /// </summary>
    public class MemoryCaches
    {
        /// <summary>
        /// MemoryCaches
        /// </summary>
        public static IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

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
    }
}