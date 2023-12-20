using Xunit;
using FluentAssertions;
namespace AoC2023;

public class Day13
{
    private Data[] Parse(string input)
    {
        return input.Replace("\r", "").Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r =>
                {
                    var verticals = r.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.AsEnumerable().ToArray()).ToArray();
                    var horizontal = Enumerable.Range(0, verticals[0].Length).Select(j => Enumerable.Range(0, verticals.Length).Select(i => verticals[i][j]).ToArray()).ToArray();
                    return new Data(verticals, horizontal);
                })
                .ToArray();
    }

    record Data(char[][] Horizontal, char[][] Vertical);

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = Parse(input);

        var acc = data.Select(d =>
        {
            var ret = 0L;
            for (int i = 1; i < d.Horizontal[0].Length; i++)
            {
                if (d.Horizontal.All(s => IsRefected(i - 1, i, s)))
                {
                    ret += i;
                    break;
                }
            }

            for (int i = 1; i < d.Vertical[0].Length; i++)
            {
                if (d.Vertical.All(s => IsRefected(i - 1, i, s)))
                {
                    ret += 100 * i;
                    break;
                }
            }

            return ret;
        }).ToArray();
        var ret = acc.Sum();

        ret.Should().Be(expected);
    }

    private bool IsRefected(int start, int end, char[] str)
    {
        while (true)
        {
            if (start < 0 || end == str.Length)
                return true;
            if (str[start] != str[end])
                return false;
            start--;
            end++;
        }
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            #.##..##.
                            ..#.##.#.
                            ##......#
                            ##......#
                            ..#.##.#.
                            ..##..##.
                            #.#.##.#.

                            #...##..#
                            #....#..#
                            ..##..###
                            #####.##.
                            #####.##.
                            ..##..###
                            #....#..#
                            """,
                            405
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day13.txt"),
                            30535
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = Parse(input);

        var acc = data.Select((d, t) =>
        {
            var ret = 0L;
            for (int i = 0; i < d.Horizontal.Length; i++)
            {
                for (int j = 0; j < d.Vertical.Length; j++)
                {
                    var originalSymbol = d.Vertical[j][i];
                    var newSymbol = originalSymbol == '.' ? '#' : '.';

                    d.Vertical[j][i] = newSymbol;
                    for (int k = 1; k < d.Horizontal.Length; k++)
                    {
                        if (d.Vertical.All(s => IsRefected(k - 1, k, s, i)))
                        {
                            ret += 100 * k;
                            break;
                        }
                    }
                    d.Vertical[j][i] = originalSymbol;

                    d.Horizontal[i][j] = newSymbol;
                    for (int k = 1; k < d.Vertical.Length; k++)
                    {
                        if (d.Horizontal.All(s => IsRefected(k - 1, k, s, j)))
                        {
                            ret += k;
                            break;
                        }
                    }
                    d.Horizontal[i][j] = originalSymbol;
                }
            }

            return ret / 2;
        }).ToArray();
        var ret = acc.Sum();

        ret.Should().Be(expected);
    }

    private bool IsRefected(int start, int end, char[] str, int index)
    {
        bool hadIndex = false;
        while (true)
        {
            if (start < 0 || end == str.Length)
                return hadIndex;
            
            hadIndex = hadIndex || start == index || end == index;
            
            if (str[start] != str[end])
                return false;
            start--;
            end++;
        }
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
        {
            new object[]
                        {
                            """
                            #.##..##.
                            ..#.##.#.
                            ##......#
                            ##......#
                            ..#.##.#.
                            ..##..##.
                            #.#.##.#.

                            #...##..#
                            #....#..#
                            ..##..###
                            #####.##.
                            #####.##.
                            ..##..###
                            #....#..#
                            """,
                            400
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day13.txt"),
                            30844
                        },
       };
}
