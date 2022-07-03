using System;
using System.Collections.Immutable;

namespace KellySharp;

public static class PrisonerRiddle
{
    public static int[] PrepareBoxes(int count, Random random)
    {
        var result = new int[count];
        for (int i = 0; i < count; ++i)
            result[i] = i;
        return result;
    }

    public static int CountSuccessfulPrisoners(ReadOnlySpan<int> boxes, IPrisoner prisoner)
    {
        var result = 0;
        var guessCount = boxes.Length / 2;

        for (int i = 0; i < boxes.Length; ++i)
        {
            var guess = prisoner.GetFirstGuess(i, boxes.Length);
            var box = boxes[guess];

            if (box == i)
            {
                ++result;
                continue;
            }

            for (int j = 1; j < guessCount; ++j)
            {
                guess = prisoner.GetNextGuess(box);
                box = boxes[guess];

                if (box == i)
                {
                    ++result;
                    break;
                }
            }
        }

        return result;
    }
}
