using Xunit;
using FluentAssertions;

namespace AoC2025;

public class Day2
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var ret = 0L;
        var rows = input.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var range in rows)
        {
            var rangeSplit = range.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
            var left = long.Parse(rangeSplit[0]);
            var right = long.Parse(rangeSplit[1]);
            for (var pow = (rangeSplit[0].Length + 1) / 2; pow <= rangeSplit[1].Length / 2; pow++)
            {
                var inc = Enumerable.Range(0, pow).Aggregate(1L, (curr, next) => curr * 10);
                var limit = inc * inc;
                var curr = limit / 10;
                while(curr < limit)
                {
                    var candidate = curr + curr / inc;
                    if (candidate >= left)
                    {
                        if (candidate > right)
                        {
                            break;
                        }
                        ret += candidate;
                    }
                    curr += inc;
                }
            }
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124",
                            1227775554
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day2.txt"),
                            12850231731
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var ret = 0L;
        var rows = input.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var range in rows)
        {
            var rangeSplit = range.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
            var left = long.Parse(rangeSplit[0]);
            var right = long.Parse(rangeSplit[1]);
            var invalidIds = new HashSet<long>(); 
            for (var pow = 1; pow <= rangeSplit[1].Length / 2; pow++)
            {
                var inc = Enumerable.Range(0, pow).Aggregate(1L, (curr, next) => curr * 10);
                for (var i = inc / 10; i < inc; i++)
                {
                    var candidate = i * inc + i;
                    while (candidate < left)
                    {
                        candidate = candidate * inc + i;
                    }
                    while (candidate <= right)
                    {
                        if (!invalidIds.Contains(candidate))
                        {
                            ret += candidate;
                            invalidIds.Add(candidate);
                        }
                        candidate = candidate * inc + i;
                    }
                }
            }
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
             new object[]
                        {
                            "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124",
                            4174379265L
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day2.txt"),
                            24774350322L
                        },
    };
}
