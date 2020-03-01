using System;
using System.Threading;
using System.Threading.Tasks;

namespace KellySharp
{
    public readonly struct AutoSemaphore : IDisposable
    {
        public static async Task<AutoSemaphore> WaitAsync(SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            return new AutoSemaphore(semaphore);
        }
        
        private readonly SemaphoreSlim _semaphore;

        public AutoSemaphore(SemaphoreSlim semaphore) => _semaphore = semaphore;
        public void Dispose() => _semaphore.Release();
    }
}