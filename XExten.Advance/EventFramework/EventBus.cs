using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XExten.Advance.EventFramework.EventContext;
using XExten.Advance.EventFramework.EventSources;
using XExten.Advance.EventFramework.PublishEvent;
using XExten.Advance.EventFramework.SubscriptEvent;
using XExten.Advance.IocFramework;

namespace XExten.Advance.EventFramework
{
    /// <summary>
    /// 事件总线
    /// </summary>
    public sealed class EventBus
    {
        /// <summary>
        /// 初始化加载
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public static Task Lancher(params Assembly[] inputs)
        {
            return OnLancher(inputs);
        }

        private static Task OnLancher(params Assembly[] inputs)
        {
            IocDependency.Register<IEventChangeStore, EventChangeStore>();
            IocDependency.Register<IEventPublish, EventPublish>();

            var subscribers = inputs.SelectMany(x => x.GetExportedTypes().Where(t => t.IsPublic && t.IsClass && !t.IsInterface && !t.IsAbstract && typeof(IEventSubscriber).IsAssignableFrom(t)));
            var bindingAttr = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            HashSet<EventExcutor> eventHandler = new HashSet<EventExcutor>();

            foreach (Type subscriber in subscribers)
            {
                IocDependency.Register(typeof(IEventSubscriber), subscriber);

                var eventHandlerMethods = subscriber.GetMethods(bindingAttr)
                    .Where(u => u.IsDefined(typeof(EventSubscribeAttribute), false));

                foreach (MethodInfo eventHandlerMethod in eventHandlerMethods)
                {
                    var handler = eventHandlerMethod.CreateDelegate(typeof(Func<IEventSource, Task>), IocDependency.Resolve(subscriber));

                    var eventSubscribeAttributes = eventHandlerMethod.GetCustomAttributes<EventSubscribeAttribute>(false);

                    foreach (var eventSubscribeAttribute in eventSubscribeAttributes)
                    {
                        eventHandler.Add(new EventExcutor(eventSubscribeAttribute.EventId)
                        {
                            Handler = handler
                        });
                    }
                }
            }

            Timer timer = new Timer(async _ =>
            {
                var source = await IocDependency.Resolve<IEventChangeStore>().ReadAsync();
                var runs = eventHandler.Where(t => t.ShouldRun(source.EventId));

                if (!runs.Any())
                    return;

                // 创建一个任务工厂并保证执行任务都使用当前的计划程序
                var taskFactory = new TaskFactory(TaskScheduler.Current);

                foreach (var run in runs)
                {
                    run.Handler.DynamicInvoke(source);
                }
            }, null, 0, 100);

            return Task.CompletedTask;
        }
    }
}
