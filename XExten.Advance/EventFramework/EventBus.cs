using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XExten.Advance.EventFramework.EventContext;
using XExten.Advance.EventFramework.EventSources;
using XExten.Advance.EventFramework.PublishEvent;
using XExten.Advance.EventFramework.SubscriptEvent;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;
using XExten.Advance.ThreadFramework;

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
        public static Task Lancher(params Assembly[] inputs)=> OnLancher(inputs);

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
            ThreadFactory.Instance.StartWithRestart(async () =>
            {
                var source = await IocDependency.Resolve<IEventChangeStore>().ReadAsync();
                var runs = eventHandler.Where(t => t.ShouldRun(source.EventId));
                if (!runs.Any())
                    return;
                runs.ForEnumerEach(run => run.Handler.DynamicInvoke(source));
            }, "XExten.Advance.EventFramework");

            return Task.CompletedTask;
        }
    }
}
