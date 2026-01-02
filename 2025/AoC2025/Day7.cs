using Xunit;
using FluentAssertions;

namespace AoC2025;

public class Day7
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var rows = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.ToArray())
            .ToArray();
        var ret = 0L;

        for (var i = 1; i < rows.Length; i ++)
        {
            for (var j = 0; j < rows[i].Length; j ++)
            {
                if (rows[i - 1][j] == '|' || rows[i - 1][j] == 'S')
                {
                    switch(rows[i][j])
                    {
                        case '.': 
                            rows[i][j] = '|';
                            break;
                        case '^':
                            ret++;
                            if (j > 0 && rows[i][j - 1] != '^')
                            {
                                rows[i][j - 1] = '|';
                            }
                            if (j < rows[i].Length - 1 && rows[i][j + 1] != '^')
                            {
                                rows[i][j + 1] = '|';
                            }
                            break;
                    }
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
                            """
                            .......S.......
                            ...............
                            .......^.......
                            ...............
                            ......^.^......
                            ...............
                            .....^.^.^.....
                            ...............
                            ....^.^...^....
                            ...............
                            ...^.^...^.^...
                            ...............
                            ..^...^.....^..
                            ...............
                            .^.^.^.^.^...^.
                            ...............
                            """,
                            21
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day7.txt"),
                            1640L
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var rows = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.ToArray())
            .ToArray();
        var cache = new Dictionary<(int, int), long>();

        for (var j = 0; j < rows[0].Length; j ++)
        {
            if (rows[0][j] == 'S')
            {
                cache[(0, j)] = 1;
            }
        }

        for (var i = 1; i < rows.Length; i ++)
        {
            for (var j = 0; j < rows[i].Length; j ++)
            {
                if (rows[i - 1][j] == '|' || rows[i - 1][j] == 'S')
                {
                    switch(rows[i][j])
                    {
                        case '.': 
                            rows[i][j] = '|';
                            cache[(i, j)] = cache[(i - 1, j)];
                            break;
                        case '|': 
                            cache[(i, j)] += cache[(i - 1, j)];
                            break;
                        case '^':
                            if (j > 0 && rows[i][j - 1] != '^')
                            {
                                cache.TryGetValue((i, j - 1), out var val);
                                cache[(i, j - 1)] = val +  cache[(i - 1, j)];
                                rows[i][j - 1] = '|';
                            }
                            if (j < rows[i].Length - 1 && rows[i][j + 1] != '^')
                            {
                                cache[(i, j + 1)] = cache[(i - 1, j)];
                                rows[i][j + 1] = '|';
                            }
                            break;
                    }
                }
            }
        }

        var ret = 0L;
        for (var j = 0; j < rows.Last().Length; j ++)
        {
            if (rows.Last()[j] == '|')
            {
                ret += cache[(rows.Length - 1, j)];
            }
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            .......S.......
                            ...............
                            .......^.......
                            ...............
                            ......^.^......
                            ...............
                            .....^.^.^.....
                            ...............
                            ....^.^...^....
                            ...............
                            ...^.^...^.^...
                            ...............
                            ..^...^.....^..
                            ...............
                            .^.^.^.^.^...^.
                            ...............
                            """,
                            40
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day7.txt"),
                            40999072541589L
                        },
    };
}
