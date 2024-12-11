using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day10
{
    (int di, int dj)[] directions = new [] {
        (1, 0),
        (-1, 0),
        (0, -1),
        (0, 1),
    };


    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var rows = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        var n = rows.Length;
        var m = rows[0].Length;
        var ret = 0;
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < m; j++) {
                var c = rows[i][j];
                if (c == '0') {
                    var queue = new Queue<(int i, int j)>();
                    queue.Enqueue((i, j));

                    for (int k = 1; k < 10; k++) {
                        var size = queue.Count;
                        var visited = new HashSet<(int i, int j)>();
                        for (int s = 0; s < size; s++) {
                            var (ci, cj) = queue.Dequeue();
                            foreach(var (di, dj) in directions) {
                                var ni = ci + di; 
                                var nj = cj + dj; 
                                if (ni >= 0 && ni < n && nj >= 0 && nj < m && rows[ni][nj] == c + k && visited.Add((ni, nj))) {
                                    queue.Enqueue((ni, nj));
                                }
                            }
                        }
                    }

                    ret += queue.Count();
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
                            10..9..
                            2...8..
                            3...7..
                            4567654
                            ...8..3
                            ...9..2
                            .....01
                            """,
                            3
                        },
            new object[]
                        {
                            """
                            89010123
                            78121874
                            87430965
                            96549874
                            45678903
                            32019012
                            01329801
                            10456732
                            """,
                            36
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day10.txt"),
                            459
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var rows = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        var n = rows.Length;
        var m = rows[0].Length;
        var ret = 0;
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < m; j++) {
                var c = rows[i][j];
                if (c == '0') {
                    var queue = new Queue<(int i, int j)>();
                    queue.Enqueue((i, j));

                    for (int k = 1; k < 10; k++) {
                        var size = queue.Count;
                        for (int s = 0; s < size; s++) {
                            var (ci, cj) = queue.Dequeue();
                            foreach(var (di, dj) in directions) {
                                var ni = ci + di; 
                                var nj = cj + dj; 
                                if (ni >= 0 && ni < n && nj >= 0 && nj < m && rows[ni][nj] == c + k) {
                                    queue.Enqueue((ni, nj));
                                }
                            }
                        }
                    }

                    ret += queue.Count();
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
                            .....0.
                            ..4321.
                            ..5..2.
                            ..6543.
                            ..7..4.
                            ..8765.
                            ..9....
                            """,
                            3
                        },
            new object[]
                        {
                            """
                            ..90..9
                            ...1.98
                            ...2..7
                            6543456
                            765.987
                            876....
                            987....
                            """,
                            13
                        },
            new object[]
                        {
                            """
                            012345
                            123456
                            234567
                            345678
                            4.6789
                            56789.
                            """,
                            227
                        },
            new object[]
                        {
                            """
                            89010123
                            78121874
                            87430965
                            96549874
                            45678903
                            32019012
                            01329801
                            10456732
                            """,
                            81
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day10.txt"),
                            1034
                        },
    };
}
