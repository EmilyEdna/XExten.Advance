using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using XExten.Advance.CacheFramework.RedisCache;
using XExten.Advance.CacheFramework.RunTimeCache;
using XExten.Advance.LinqFramework;

namespace XExten.Advance.CacheFramework
{
    /// <summary>
    ///
    /// </summary>
    public class Caches
    {

        #region Instance
        /// <summary>
        /// redis
        /// </summary>
        public static IDatabase Redis
        {
            get
            {
                if (RedisCaches.RedisConnectionString.IsNullOrEmpty())
                    throw new ArgumentNullException($"{nameof(RedisCaches.RedisConnectionString)} can't be null");
                return RedisCaches.Instance.GetDatabase();
            }
        }
        /// <summary>
        /// memoryCache
        /// </summary>
        public static IMemoryCache Memory => MemoryCaches.Cache;
        #endregion

        #region Properties

        /// <summary>
        /// Redis链接字符串
        /// </summary>
        public static string RedisConnectionString
        {
            set { RedisCaches.RedisConnectionString = value; }
        }
        #endregion Properties

        #region Sync

        /// <summary>
        /// 添加Memory缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="MinutesOrSecond"></param>
        /// <param name="UseSecond"></param>
        public static void RunTimeCacheSet<T>(string key, T value, int MinutesOrSecond = 5, bool UseSecond = false)
        {
            MemoryCaches.AddCache<T>(key, value, MinutesOrSecond, UseSecond);
        }

        /// <summary>
        /// 添加Redis缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="MinutesOrSecond"></param>
        /// <param name="UseSecond"></param>
        public static void RedisCacheSet<T>(string key, T value, int MinutesOrSecond = 5, bool UseSecond = false)
        {
            TimeSpan? exp = (UseSecond ? (DateTime.Now.AddSeconds(MinutesOrSecond) - DateTime.Now) : (DateTime.Now.AddMinutes(MinutesOrSecond) - DateTime.Now));
            RedisCaches.StringSet<T>(key, value, MinutesOrSecond==0?null:exp);
        }

        /// <summary>
        /// 获取Memory缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T RunTimeCacheGet<T>(object key)
        {
            return MemoryCaches.GetCache<T>(key.ToString());
        }

        /// <summary>
        /// 获取Redis缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T RedisCacheGet<T>(object key)
        {
            return RedisCaches.StringGet<T>(key.ToString());
        }
  
        /// <summary>
        ///  删除Memory缓存
        /// </summary>
        /// <param name="key"></param>
        public static void RunTimeCacheRemove(object key)
        {
            MemoryCaches.RemoveCache(key.ToString());
        }

        /// <summary>
        /// 删除Redis缓存
        /// </summary>
        /// <param name="key"></param>
        public static void RedisCacheRemove(object key)
        {
            RedisCaches.KeyDelete(key.ToString());
        }    
        #endregion Sync

        #region Async

        /// <summary>
        /// 添加Memory缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="MinutesOrSecond"></param>
        /// <param name="UseSecond"></param>
        /// <returns></returns>
        public static async Task RunTimeCacheSetAsync<T>(string key, T value, int MinutesOrSecond = 5, bool UseSecond = false)
        {
            await Task.Run(() => RunTimeCacheSet(key, value, MinutesOrSecond, UseSecond));
        }

        /// <summary>
        /// 添加Redis缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="MinutesOrSecond"></param>
        /// <param name="UseSecond"></param>
        /// <returns></returns>
        public static async Task RedisCacheSetAsync<T>(string key, T value, int MinutesOrSecond = 5, bool UseSecond = false)
        {
            TimeSpan? exp = (UseSecond ? (DateTime.Now.AddSeconds(MinutesOrSecond) - DateTime.Now) : (DateTime.Now.AddMinutes(MinutesOrSecond) - DateTime.Now));
            await RedisCaches.StringSetAsync<T>(key, value, MinutesOrSecond == 0 ? null : exp);
        }
        /// <summary>
        /// 获取Memory缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<T> RunTimeCacheGetAsync<T>(object key)
        {
            return await Task.Run(() => RunTimeCacheGet<T>(key));
        }

        /// <summary>
        /// 获取Redis缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<T> RedisCacheGetAsync<T>(object key)
        {
            return await RedisCaches.StringGetAsync<T>(key.ToString());
        }

        /// <summary>
        ///  删除Memory缓存
        /// </summary>
        /// <param name="key"></param>
        public static async Task RunTimeCacheRemoveAsync(object key)
        {
            await Task.Run(() => RunTimeCacheRemove(key));
        }

        /// <summary>
        /// 删除Redis缓存
        /// </summary>
        /// <param name="key"></param>
        public static async Task RedisCacheRemoveAsync(object key)
        {
            await RedisCaches.KeyDeleteAsync(key.ToString());
        }

        #endregion Async
    }
}