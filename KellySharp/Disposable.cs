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

    public static class Disposable
    {
        public static Disposable<T> Create<T>(T data, Action<T> action) =>
            new Disposable<T>(data, action);
    }
}