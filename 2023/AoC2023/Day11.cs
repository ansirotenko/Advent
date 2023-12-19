using Xunit;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
namespace AoC2023;

public class Day11
{
    private string[] Parse(string input)
    {
        return input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
    }

    record Point(int I, int J);


    private long GetAnswer(string input, int multiplier)
    {
        var data = Parse(input);
        var processed = new List<Point>();
        var n = data.Length;
        var m = data[0].Length;
        var emptyis = GetEmptyIs(data, n, m);
        var emptyjs = GetEmptyJs(data, n, m);
        var ret = 0L;

        var ei = 0;
        var reali = 0;
        for (int i = 0; i < n; i++)
        {
            reali++;
            if (ei < emptyis.Count && emptyis[ei] == i)
            {
                ei++;
                reali += multiplier - 1;
            }
            else
            {
                var ej = 0;
                int realj = 0;
                for (int j = 0; j < m; j++)
                {
                    realj++;
                    if (ej < emptyjs.Count && emptyjs[ej] == j)
                    {
                        ej++;
                        realj += multiplier - 1;
                    }
                    if (data[i][j] == '#')
                    {
                        ret = processed.Aggregate(ret, (acc, curr) =>
                        {
                            return acc + Math.Abs(curr.I - reali) + Math.Abs(curr.J - realj);
                        });
                        processed.Add(new Point(reali, realj));
                    }
                }
            }
        }

        return ret;
    }

    private List<int> GetEmptyJs(string[] data, int n, int m)
    {
        var ret = new List<int>();
        for (int j = 0; j < m; j++)
        {
            var isEmpty = true;
            for (int i = 0; i < n; i++)
            {
                if (data[i][j] != '.')
                {
                    isEmpty = false;
                    break;
                }
            }
            if (isEmpty)
            {
                ret.Add(j);
            }
        }
        return ret;
    }

    private List<int> GetEmptyIs(string[] data, int n, int m)
    {
        var ret = new List<int>();
        for (int i = 0; i < n; i++)
        {
            var isEmpty = true;
            for (int j = 0; j < m; j++)
            {
                if (data[i][j] != '.')
                {
                    isEmpty = false;
                    break;
                }
            }
            if (isEmpty)
            {
                ret.Add(i);
            }
        }
        return ret;
    }


    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var ret = GetAnswer(input, 2);
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            ...#......
                            .......#..
                            #.........
                            ..........
                            ......#...
                            .#........
                            .........#
                            ..........
                            .......#..
                            #...#.....
                            """,
                            374
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day11.txt"),
                            10422930
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected, int multiplier)
    {
        var ret = GetAnswer(input, multiplier);
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
        {
            new object[]
                        {
                            """
                            ...#......
                            .......#..
                            #.........
                            ..........
                            ......#...
                            .#........
                            .........#
                            ..........
                            .......#..
                            #...#.....
                            """,
                            1030,
                            10
                        },
            new object[]
                        {
                            """
                            ...#......
                            .......#..
                            #.........
                            ..........
                            ......#...
                            .#........
                            .........#
                            ..........
                            .......#..
                            #...#.....
                            """,
                            8410,
                            100
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day11.txt"),
                            -1,
                            1000000
                        },
        };
}
