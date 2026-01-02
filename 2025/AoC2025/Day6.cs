using Xunit;
using FluentAssertions;

namespace AoC2025;

public class Day6
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var rows = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var numbers = rows.Take(rows.Length - 1)
            .Select(row => row.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray())
            .ToArray();
        var op = rows.Last().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.First()).ToArray();

        var ret = 0L;
        for (var i = 0; i < op.Length; i++)
        {
            ret += numbers.Skip(1).Aggregate(numbers.First()[i], (curr, next) => op[i] == '+' ? curr + next[i] : curr * next[i]);
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            123 328  51 64 
                            45 64  387 23 
                            6 98  215 314
                            *   +   *   +  
                            """,
                            4277556
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day6.txt"),
                            4583860641327L
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var rows = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        var ret = 0L;
        var j = 0;
        while (j < rows.Last().Length)
        {
            var jstart = j;
            j++;
            while (j < rows.Last().Length && rows.Last()[j] == ' ')
            {
                j++;
            }
            var jend = j == rows.Last().Length ? j : j - 1;

            var numbers = Enumerable.Range(jstart, jend - jstart)
                .Select(index => rows
                                .Take(rows.Length - 1)
                                .Where(x => x[index] != ' ')
                                .Aggregate(0L, (curr, next) => curr * 10 + next[index] - '0')
                                )
                .ToArray();

            ret += numbers.Skip(1).Aggregate(numbers.First(), (curr, next) => rows.Last()[jstart] == '+' ? curr + next : curr * next);
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                           """
                            123 328  51 64 
                             45 64  387 23 
                              6 98  215 314
                            *   +   *   +  
                            """,
                            3263827
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day6.txt"),
                            11602774058280L
                        },
    };
}
