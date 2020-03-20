using System;
using System.Threading.Tasks;

namespace KellySharp
{
    public readonly struct AsyncDisposable<T> : IAsyncDisposable
    {
        private readonly T _data;
        private readonly Func<T, ValueTask> _action;

        public AsyncDisposable(T data, Func<T, ValueTask> action)
        {
            _data = data;
            _action = action;
        }
        
        public ValueTask DisposeAsync() => _action(_data);
    }

    public readonly struct AsyncDisposable : IAsyncDisposable
    {
        public static AsyncDisposable<T> Create<T>(T data, Func<T, ValueTask> action) =>
            new AsyncDisposable<T>(data, action);

        private readonly Func<ValueTask> _action;

        public AsyncDisposable(Func<ValueTask> action) => _action = action;

        public ValueTask DisposeAsync() => _action();
    }
}