using System;
using System.Text;
using Xunit;

namespace KellySharp.Test;

public class StringExtensionsTest
{
    private static string Repeat(ReadOnlySpan<char> text, int count)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < count; ++i)
            sb.Append(text);

        return sb.ToString();
    }

    [Theory]
    [InlineData("asdf", 11)]
    [InlineData("A decently long sentence here. Let's do this.", 22)]
    [InlineData("a", 128)]
    public void StringRepeated(string text, int count)
    {
        var expected = Repeat(text, count);
        var actual = text.Repeated(count);
        Assert.Equal(expected, actual);
    }
}
