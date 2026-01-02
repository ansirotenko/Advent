using Xunit;
using FluentAssertions;

namespace AoC2025;

public class Day3
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var ret = 0L;
        var rows = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in rows)
        {
            var bank = line.Select(c => c - '0').ToList();
            var lmax = bank.Take(bank.Count - 1).Max();
            var lmaxIndex = bank.IndexOf(lmax);
            ret += lmax * 10 + bank.Skip(lmaxIndex + 1).Max();
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            987654321111111
                            811111111111119
                            234234234234278
                            818181911112111
                            """,
                            357
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day3.txt"),
                            17196
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var ret = 0L;
        var rows = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in rows)
        {
            var bank = line.Select(c => c - '0').ToList();
            var index = 0;
            var joltaje = 0L;
            for (int i = 11; i >= 0; i--)
            {
                var max = bank[index];
                for (var j = index + 1; j < bank.Count - i; j++)
                {
                    if (bank[j] > max)
                    {
                        max = bank[j];
                        index = j;
                    }
                }
                index++;
                joltaje = joltaje * 10 + max;

            }
            ret += joltaje;
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
             new object[]
                        {
                            """
                            987654321111111
                            811111111111119
                            234234234234278
                            818181911112111
                            """,
                            3121910778619L
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day3.txt"),
                            171039099596062L
                        },
    };
}
