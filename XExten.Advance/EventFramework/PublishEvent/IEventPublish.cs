using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.EventFramework.EventSources;

namespace XExten.Advance.EventFramework.PublishEvent
{
    /// <summary>
    /// 发布者
    /// </summary>
    public interface IEventPublish
    {
        /// <summary>
        /// 实例
        /// </summary>
        static IEventPublish  Instance => new EventPublish();
        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="source"></param>
        /// <returns><see cref="Task"/></returns>
        Task PublishAsync(Action<IEventSource> source);
        /// <summary>
        /// 延迟发布
        /// </summary>
        /// <param name="source"></param>
        /// <param name="delay">毫秒</param>
        /// <returns><see cref="Task"/></returns>
        Task DelayPublishAsync(Action<IEventSource> source,long delay);
    }
}
