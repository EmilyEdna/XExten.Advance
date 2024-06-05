using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using XExten.Advance.CacheFramework.RunTimeCache;

namespace XExten.Advance.CacheFramework
{
    /// <summary>
    ///
    /// </summary>
    public class Caches
    {

        #region Instance
        /// <summary>
        /// memoryCache
        /// </summary>
        public static IMemoryCache Memory => MemoryCaches.Cache;
        #endregion

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
        ///  删除Memory缓存
        /// </summary>
        /// <param name="key"></param>
        public static void RunTimeCacheRemove(object key)
        {
            MemoryCaches.RemoveCache(key.ToString());
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
        ///  删除Memory缓存
        /// </summary>
        /// <param name="key"></param>
        public static async Task RunTimeCacheRemoveAsync(object key)
        {
            await Task.Run(() => RunTimeCacheRemove(key));
        }
        #endregion Async
    }
}