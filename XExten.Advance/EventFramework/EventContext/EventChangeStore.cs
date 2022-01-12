using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using XExten.Advance.EventFramework.EventSources;

namespace XExten.Advance.EventFramework.EventContext
{
    internal class EventChangeStore : IEventChangeStore
    {
        private readonly Channel<IEventSource> Channels;
        public EventChangeStore()
        {
            this.Channels = Channel.CreateBounded<IEventSource>(new BoundedChannelOptions(3000)
            {
                FullMode = BoundedChannelFullMode.Wait
            });
        }

        public async Task<IEventSource> ReadAsync()
        {
            return await Channels.Reader.ReadAsync();
        }

        public async Task WriteAsync(IEventSource input)
        {
            await Channels.Writer.WriteAsync(input);
        }
    }
}
