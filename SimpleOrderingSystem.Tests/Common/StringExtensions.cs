
using System.Text;

namespace SimpleOrderingSystem.Tests.Common;

public static class StringExtensions
{
    public static string Repeat(this string input, int repeat)
    {
        var sb = new StringBuilder(repeat * input.Length);
        for(int i = 0; i < repeat; i++)
        {
            sb.Append(input);
        }

        return sb.ToString();
    }
}