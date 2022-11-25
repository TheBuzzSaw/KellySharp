using System;
using System.Collections.Generic;

namespace KellySharp;

public static class FisherYates
{
    public static void Shuffle<T>(Span<T> items, Random random)
    {
        var last = items.Length - 1;
        for (int i = 0; i < last; ++i)
        {
            var swapIndex = random.Next(i, items.Length);
            (items[swapIndex], items[i]) = (items[i], items[swapIndex]);
        }
    }

    public static void Shuffle<T>(int shuffleCount, Span<T> items, Random random)
    {
        for (int i = 0; i < shuffleCount; ++i)
            Shuffle(items, random);
    }

    public static void Shuffle<T>(IList<T> items, Random random)
    {
        var last = items.Count - 1;
        for (int i = 0; i < last; ++i)
        {
            var swapIndex = random.Next(i, items.Count);
            var swapValue = items[i];
            items[i] = items[swapIndex];
            items[swapIndex] = swapValue;
        }
    }

    public static void Shuffle<T>(int shuffleCount, IList<T> items, Random random)
    {
        for (int i = 0; i < shuffleCount; ++i)
            Shuffle(items, random);
    }
}
