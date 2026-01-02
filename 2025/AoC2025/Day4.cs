using Xunit;
using FluentAssertions;

namespace AoC2025;

public class Day4
{

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var ret = 0L;
        var rows = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.ToArray())
            .ToArray();
        (int, int)[] directions = {
            (1, 0),
            (0, 1),
            (-1, 0),
            (0, -1),
            (1, 1),
            (1, -1),
            (-1, 1),
            (-1, -1),
        };

        for(var i = 0; i < rows.Length; i++)
        {
            for(var j = 0; j < rows[i].Length; j++)
            {
                if (rows[i][j] == '@')
                {
                    var neighbours = 0;
                    foreach(var (di, dj) in directions)
                    {
                        if (i + di >= 0 && i + di < rows.Length && j + dj >= 0 && j + dj < rows[i].Length && rows[i + di][j + dj] == '@')
                        {
                            neighbours++;
                        }
                    }
                    if (neighbours < 4)
                    {
                        ret ++;
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
                            ..@@.@@@@.
                            @@@.@.@.@@
                            @@@@@.@.@@
                            @.@@@@..@.
                            @@.@@@@.@@
                            .@@@@@@@.@
                            .@.@.@.@@@
                            @.@@@.@@@@
                            .@@@@@@@@.
                            @.@.@@@.@.
                            """,
                            13
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day4.txt"),
                            1419
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var ret = 0L;
        var rows = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.ToArray())
            .ToArray();
        (int, int)[] directions = {
            (1, 0),
            (0, 1),
            (-1, 0),
            (0, -1),
            (1, 1),
            (1, -1),
            (-1, 1),
            (-1, -1),
        };

        var q = new Stack<(int i, int j)>();
        for(var i = 0; i < rows.Length; i++)
        {
            for(var j = 0; j < rows[i].Length; j++)
            {
                q.Push((i, j));
            }
        }

        while(q.Any())
        {
            var (i, j) = q.Pop();
            if (rows[i][j] == '@')
            {
                var neighbours = new List<(int, int)>();
                foreach(var (di, dj) in directions)
                {
                    if (i + di >= 0 && i + di < rows.Length && j + dj >= 0 && j + dj < rows[i].Length && rows[i + di][j + dj] == '@')
                    {
                        neighbours.Add((i + di, j + dj));
                    }
                }
                if (neighbours.Count < 4)
                {
                    ret++;
                    rows[i][j] = 'X';
                    foreach(var n in neighbours)
                    {
                        q.Push(n);
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
                            """
                            ..@@.@@@@.
                            @@@.@.@.@@
                            @@@@@.@.@@
                            @.@@@@..@.
                            @@.@@@@.@@
                            .@@@@@@@.@
                            .@.@.@.@@@
                            @.@@@.@@@@
                            .@@@@@@@@.
                            @.@.@@@.@.
                            """,
                            43
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day4.txt"),
                            8739
                        },
    };
}
