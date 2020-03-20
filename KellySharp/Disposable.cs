using System;

namespace KellySharp
{
    public readonly struct Disposable<T> : IDisposable
    {
        private readonly T _data;
        private readonly Action<T> _action;

        public Disposable(T data, Action<T> action)
        {
            _data = data;
            _action = action;
        }

        public void Dispose() => _action(_data);
    }

    public readonly struct Disposable : IDisposable
    {
        public static Disposable<T> Create<T>(T data, Action<T> action) =>
            new Disposable<T>(data, action);

        private readonly Action _action;

        public Disposable(Action action) => _action = action;

        public void Dispose() => _action();
    }
}