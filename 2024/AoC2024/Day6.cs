using Xunit;
using FluentAssertions;
using System.Linq.Expressions;

namespace AoC2024;

public class Day6
{
    private static (int di, int dj)[] directions = new [] {
        (1, 0), // down
        (0, -1), // left
        (-1, 0), //up
        (0, 1), // right
    };

    private static Dictionary<char, int> guardPos = new Dictionary<char, int> {
        {'v', 0}, // down
        {'<', 1}, // left
        {'^', 2}, //up
        {'>', 3}, // right
    };

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var ret = 1;
        var direction = 0;
        var i = 0;
        var j = 0;
        var map = input.Split(new []{"\n\r", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToArray())
                .ToArray();
        for (i = 0; i < map.Length; i++) {
            var spot = false;
            for (j = 0; j < map[i].Length; j++) {
                if (guardPos.TryGetValue(map[i][j], out direction)) {
                    map[i][j] = 'X';
                    spot = true;
                    break;
                }
            };
            if (spot) {
                break;
            }
        };

        while(true) {
            var nextI = i + directions[direction].di;
            var nextJ = j + directions[direction].dj;
            if (nextI < 0 || nextI >= map.Length || nextJ < 0 || nextJ >= map[i].Length) {
                break;
            }

            var c = map[nextI][nextJ];
            if (c == '#') {
                direction = (direction + 1) % directions.Length;
            } else {
                if (c == '.') {
                    ret++;
                    map[nextI][nextJ] = 'X';
                }
                i = nextI;
                j = nextJ;
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
                            ....#.....
                            .........#
                            ..........
                            ..#.......
                            .......#..
                            ..........
                            .#..^.....
                            ........#.
                            #.........
                            ......#...
                            """,
                            41
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day6.txt"),
                            5212
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var ret = 0;
        var startDirection = 0;
        var startI = 0;
        var startJ = 0;
        var map = input.Split(new []{"\n\r", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToArray())
                .ToArray();
        for (var k = 0; k < map.Length * map[0].Length; k++) {
            var i = k / map[0].Length;
            var j = k % map[0].Length;
            if (guardPos.TryGetValue(map[i][j], out startDirection)) {
                startI = i;
                startJ = j;
                break;
            }
        };

        for (var i = 0; i < map.Length; i++) {
            for (var j = 0; j < map[i].Length; j++) {
                if (map[i][j] == '.' ) {
                    map[i][j] = 'O';
                    var visited = map.Select(row => new int[row.Length]).ToArray();
                    var curI = startI;
                    var curJ = startJ;
                    var currDirection = startDirection;
                    visited[curI][curJ] = (int)Math.Pow(2, currDirection);
                    while(true) {
                        var nextI = curI + directions[currDirection].di;
                        var nextJ = curJ + directions[currDirection].dj;
                        if (nextI < 0 || nextI >= map.Length || nextJ < 0 || nextJ >= map[curI].Length) {
                            break;
                        }

                        var c = map[nextI][nextJ];
                        if (c == '#' || c == 'O') {
                            currDirection = (currDirection + 1) % directions.Length;
                        } else {
                            curI = nextI;
                            curJ = nextJ;
                        }

                        int visitedDir = (int)Math.Pow(2, currDirection);
                        if ((visited[curI][curJ] & visitedDir) != 0) {
                            ret++;
                            break;
                        }
                        visited[curI][curJ] |= visitedDir;
                    }
                    map[i][j] = '.';
                }
            };
        };

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            ....#.....
                            .........#
                            ..........
                            ..#.......
                            .......#..
                            ..........
                            .#..^.....
                            ........#.
                            #.........
                            ......#...
                            """,
                            6
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day6.txt"),
                            1767
                        },
    };
}
