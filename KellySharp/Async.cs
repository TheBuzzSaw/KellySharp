using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace KellySharp;

public static class Async
{
    public static Task ChunkAsync<T>(
        int maxConcurrency,
        IEnumerable<T> values,
        Func<T, Task> taskGenerator,
        CancellationToken cancellationToken = default)
    {
        if (maxConcurrency < 2)
            return RunAllAsync(values, taskGenerator, cancellationToken);
        else
            return RunAllAsync(maxConcurrency, values, taskGenerator, cancellationToken);
    }

    private static async Task RunAllAsync<T>(
        IEnumerable<T> values,
        Func<T, Task> taskGenerator,
        CancellationToken cancellationToken = default)
    {
        foreach (var value in values)
        {
            var task = taskGenerator.Invoke(value);
            await task;
        }
    }

    private static async Task RunAllAsync<T>(
        int maxConcurrency,
        IEnumerable<T> values,
        Func<T, Task> taskGenerator,
        CancellationToken cancellationToken = default)
    {
        var options = new BoundedChannelOptions(1)
        {
            SingleWriter = true,
            SingleReader = false,
            FullMode = BoundedChannelFullMode.Wait,
            AllowSynchronousContinuations = false
        };

        var channel = Channel.CreateBounded<Task>(options);
        var writer = channel.Writer;

        var consumers = new Task[maxConcurrency - 1];

        for (int i = 0; i < consumers.Length; ++i)
            consumers[i] = ConsumeAsync(channel.Reader, cancellationToken);

        foreach (var value in values)
        {
            var wait = await writer.WaitToWriteAsync(cancellationToken);

            if (!wait)
                break; // Should be impossible. ¯\_(ツ)_/¯
            
            var task = taskGenerator.Invoke(value);
            await writer.WriteAsync(task, cancellationToken);
        }

        writer.Complete();

        foreach (var consumer in consumers)
            await consumer;
    }

    private static async Task ConsumeAsync(
        ChannelReader<Task> channelReader,
        CancellationToken cancellationToken)
    {
        await foreach (var task in channelReader.ReadAllAsync(cancellationToken))
            await task;
    }
}
