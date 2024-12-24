using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day11
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var numbers = input.Split(new []{" ", "\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);
        var cache = new Dictionary<int, Dictionary<string, long>>();
        var ret = numbers.Sum(num => Solve(num, 25, cache));
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            "125 17",
                            55312L
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day11.txt"),
                            191690L
                        },
    };

    private static long Solve(string number, int depth, Dictionary<int, Dictionary<string, long>> cache) {
        if (depth == 0) {
            return 1L;
        }
        if (!cache.TryGetValue(depth, out var depthCache)) {
            depthCache = new Dictionary<string, long>();
            cache[depth] = depthCache;
        }
        if (depthCache.TryGetValue(number, out var ret)) {
            return ret;
        }
        if (number == "0") {
            ret = Solve("1", depth - 1, cache);
        } else {
            var n = number.Length;
            if (n % 2 == 0) {
                var left = number.Substring(0, n / 2);
                var right = number.Substring(n / 2, n / 2);
                ret = Solve(left, depth - 1, cache) + Solve(long.Parse(right).ToString(), depth -1, cache);
            } else {
                ret = Solve((long.Parse(number) * 2024).ToString(), depth - 1, cache);
            }
        }
        depthCache[number] = ret;
        return ret;
    }

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var numbers = input.Split(new []{" ", "\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);
        var cache = new Dictionary<int, Dictionary<string, long>>();
        var ret = numbers.Sum(num => Solve(num, 75, cache));
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            File.ReadAllText("Inputs/Day11.txt"),
                            228651922369703L
                        },
    };
}
