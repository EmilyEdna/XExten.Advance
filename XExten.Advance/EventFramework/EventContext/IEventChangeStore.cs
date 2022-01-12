using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.EventFramework.EventSources;

namespace XExten.Advance.EventFramework.EventContext
{
    internal interface IEventChangeStore
    {
        Task WriteAsync(IEventSource input);

        Task<IEventSource> ReadAsync();
    }
}
