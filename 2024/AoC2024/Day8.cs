using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day8
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var map = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        var antinodes = map.Select(row => new bool[row.Length]).ToArray();
        var ret = 0;

        int i = 0;
        int j = 0;
        int n = map.Length;
        int m = map[0].Length;
        for (int k = 0; k < n*m; k++) {
            var c = map[k / m][k % m];
            if (c != '.') {
                for (int p = k + 1; p < n*m; p++) {
                    if (c == map[p / m][p % m]) {
                        var deltaI = p / m - k / m;
                        var deltaJ = p % m - k % m;

                        i = k / m - deltaI;
                        j = k % m - deltaJ;
                        if (i >= 0 && i < n && j >= 0 && j < m && !antinodes[i][j]) {
                            ret++;
                            antinodes[i][j] = true;
                        }

                        i = p / m + deltaI;
                        j = p % m + deltaJ;
                        if (i >= 0 && i < n && j >= 0 && j < m && !antinodes[i][j]) {
                            ret++;
                            antinodes[i][j] = true;
                        }
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
                            ............
                            ........0...
                            .....0......
                            .......0....
                            ....0.......
                            ......A.....
                            ............
                            ............
                            ........A...
                            .........A..
                            ............
                            ............
                            """,
                            14
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day8.txt"),
                            295
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var map = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        var antinodes = map.Select(row => new bool[row.Length]).ToArray();
        var ret = 0;

        int i = 0;
        int j = 0;
        int n = map.Length;
        int m = map[0].Length;
        for (int k = 0; k < n*m; k++) {
            var c = map[k / m][k % m];
            if (c != '.') {
                for (int p = k + 1; p < n*m; p++) {
                    if (c == map[p / m][p % m]) {
                        var deltaI = p / m - k / m;
                        var deltaJ = p % m - k % m;

                        i = p / m;
                        j = p % m;
                        while(i >= 0 && i < n && j >= 0 && j < m) {
                            if (!antinodes[i][j]) {
                                ret++;
                                antinodes[i][j] = true;
                            }
                            i -= deltaI;
                            j -= deltaJ;
                        }

                        i = k / m;
                        j = k % m;
                        while(i >= 0 && i < n && j >= 0 && j < m) {
                            if (!antinodes[i][j]) {
                                ret++;
                                antinodes[i][j] = true;
                            }
                            i += deltaI;
                            j += deltaJ;
                        }
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
                            T.........
                            ...T......
                            .T........
                            ..........
                            ..........
                            ..........
                            ..........
                            ..........
                            ..........
                            ..........
                            """,
                            9
                        },
            new object[]
                        {
                            """
                            ............
                            ........0...
                            .....0......
                            .......0....
                            ....0.......
                            ......A.....
                            ............
                            ............
                            ........A...
                            .........A..
                            ............
                            ............
                            """,
                            34
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day8.txt"),
                            1034
                        },
    };
}
