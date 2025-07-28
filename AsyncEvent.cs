using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncEvent
{
    public sealed class AsyncEvent<TEventArgs> where TEventArgs : EventArgs
    {
        public delegate Task AsyncEventHandler(object sender, TEventArgs e);

        private readonly List<AsyncEventHandler> handlers = [];

        public void AddHandler(AsyncEventHandler handler) => handlers.Add(handler);
        public void RemoveHandler(AsyncEventHandler handler) => handlers.Remove(handler);
        public void Clear() => handlers.Clear();

        public Task InvokeParallelAsync(object sender, TEventArgs args) =>
            Task.WhenAll(handlers.Select(h => h(sender, args)));

        public async Task InvokeAsync(object sender, TEventArgs args)
        {
            foreach (var h in handlers)
                await h(sender, args);
        }

        public static AsyncEvent<TEventArgs> operator +(AsyncEvent<TEventArgs> e, AsyncEventHandler handler)
        {
            e.AddHandler(handler);
            return e;
        }

        public static AsyncEvent<TEventArgs> operator -(AsyncEvent<TEventArgs> e, AsyncEventHandler handler)
        {
            e.RemoveHandler(handler);
            return e;
        }
    }
}
