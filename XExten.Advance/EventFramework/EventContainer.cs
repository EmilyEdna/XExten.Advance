using System;
using System.Collections.Generic;
using System.Text;
using DryIoc;

namespace XExten.Advance.EventFramework
{
    internal sealed class EventContainer
    {
        private static EventContainer _Instance;
        private static IContainer Container;
        private static readonly object locker = new object();

        internal static EventContainer Instance
        {
            get
            {
                lock (locker)
                {
                    if (_Instance == null)
                    {
                        _Instance = new EventContainer();
                        Container = new Container();
                    }
                }
                return _Instance;
            }
        }

        internal void Regiest<TService, TImplementation>() where TImplementation : TService
        {
            Container.Register<TService, TImplementation>(Reuse.Singleton);
        }

        internal void Regiest(Type serviceType, Type implementationType)
        {
            Container.Register(serviceType, implementationType, Reuse.Singleton);
        }

        internal object Resolve(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }

        internal TService Resolve<TService>()
        {
            return Container.Resolve<TService>();
        }
    }
}
