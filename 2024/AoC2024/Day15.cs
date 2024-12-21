using Xunit;
using FluentAssertions;
using System.Text;
using System.Diagnostics;

namespace AoC2024;

public class Day15
{
    class Direction {
        public int I { get; }
        public int J { get; }

        private Direction(int i, int j) {
            I = i;
            J = j;
        }

        public static Direction Up = new Direction(-1, 0);
        public static Direction Down = new Direction(1, 0);
        public static Direction Left = new Direction(0, -1);
        public static Direction Right = new Direction(0, 1);
        public static Direction[] All = new [] {Up, Down, Left, Right};

        public static Direction Parse(char move) {
            return move switch {
                '^' => Up,
                'v' => Down,
                '>' => Right,
                '<' => Left,
                _ => throw new ArgumentException() 
            };
        }
    }

    record Data(char[][] Filed, string Moves) { 
        public static Data Parse(string input) {
            var split = input.Split(new []{"\n\n", "\r\n\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            return new Data(
                split[0].Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.ToArray()).ToArray(),
                split[1].Replace("\r\n", "").Replace("\n", "")
            );
        }
    }

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var data = Data.Parse(input);
        int n = data.Filed.Length;
        int m = data.Filed[0].Length;
        int i = 0;
        int j = 0;
        for (int k = 0; k < n * m; k++) {
            i = k / m;
            j = k % m;
            if (data.Filed[i][j] == '@') {
                break;
            }
        }

        foreach(var move in data.Moves) {
            var direction = Direction.Parse(move);
            var ci = i;
            var cj = j;
            while(data.Filed[ci][cj] != '#' && data.Filed[ci][cj] != '.') {
                ci += direction.I;
                cj += direction.J;
            }
            if (data.Filed[ci][cj] == '.') {
                do {
                    var ni = ci - direction.I;
                    var nj = cj - direction.J;
                    var tmp = data.Filed[ni][nj];
                    data.Filed[ni][nj] = data.Filed[ci][cj];
                    data.Filed[ci][cj] = tmp;
                    ci = ni;
                    cj = nj;
                } while(ci != i || cj != j);
                i += direction.I;
                j += direction.J;
            }
        }

        var ret = 0;
        for (int k = 0; k < n * m; k++) {
            i = k / m;
            j = k % m;
            if (data.Filed[i][j] == 'O') {
                ret += i * 100 + j;
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
                            ########
                            #..O.O.#
                            ##@.O..#
                            #...O..#
                            #.#.O..#
                            #...O..#
                            #......#
                            ########

                            <^^>>>vv<v>>v<<
                            """,
                            2028
                        },            
            new object[]
                        {
                            """
                            ##########
                            #..O..O.O#
                            #......O.#
                            #.OO..O.O#
                            #..O@..O.#
                            #O#..O...#
                            #O..O..O.#
                            #.OO.O.OO#
                            #....O...#
                            ##########

                            <vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
                            vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
                            ><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
                            <<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
                            ^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
                            ^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
                            >^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
                            <><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
                            ^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
                            v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^
                            """,
                            10092
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day15.txt"),
                            1457740
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var data = Data.Parse(input);
        data = data with {
            Filed = data.Filed
                .Select(x => new string(x)
                            .Replace("#", "##")
                            .Replace(".", "..")
                            .Replace("@", "@.")
                            .Replace("O", "[]")
                            .ToArray()
                        )
                .ToArray()
        };
        int n = data.Filed.Length;
        int m = data.Filed[0].Length;
        int i = 0;
        int j = 0;
        for (int k = 0; k < n * m; k++) {
            i = k / m;
            j = k % m;
            if (data.Filed[i][j] == '@') {
                break;
            }
        }
        var t = -1;
        PrintField(t, ' ');
        foreach(var move in data.Moves) {
            t++;
            var direction = Direction.Parse(move);
            var layers = new List<HashSet<(int i, int j)>>{
                new HashSet<(int i, int j)>{ (i, j)}
            };
            while(true) {
                if (layers.First().All(x => data.Filed[x.i][x.j] == '.')) {
                    foreach(var layer in layers.Skip(1)) {
                        foreach(var p in layer) {
                            var ni = p.i + direction.I;
                            var nj = p.j + direction.J;
                            if (data.Filed[p.i][p.j] != '.') {
                                data.Filed[ni][nj] = data.Filed[p.i][p.j];
                                data.Filed[p.i][p.j] = '.';
                            }
                        }
                    }
                    i += direction.I;
                    j += direction.J;
                    PrintField(t, move);
                    break;
                } else {
                    if (layers.First().Any(x => data.Filed[x.i][x.j] == '#')) {
                        PrintField(t, move);
                        break;
                    } else {
                        if (direction == Direction.Left || direction == Direction.Right) {
                            layers.Insert(0, layers.First().Select(x => (x.i + direction.I, x.j + direction.J)).ToHashSet());
                        } else {
                            var newLayer = new HashSet<(int i, int j)>();
                            foreach(var p in layers.First().Where(x => data.Filed[x.i][x.j] != '.')) {
                                var ni = p.i + direction.I;
                                var nj = p.j + direction.J;
                                newLayer.Add((ni, nj));
                                if (data.Filed[ni][nj] == ']') {
                                    newLayer.Add((ni, nj - 1));
                                } else {
                                    if (data.Filed[ni][nj] == '[') {
                                        newLayer.Add((ni, nj + 1));
                                    }
                                }
                            }

                            layers.Insert(0, newLayer);
                        }
                    }
                }
            }
        }

        var ret = 0;
        for (int k = 0; k < n * m; k++) {
            i = k / m;
            j = k % m;
            if (data.Filed[i][j] == '[') {
                ret += i * 100 + j;
            }
        }
        PrintField(t, ' ');
        void PrintField(int counter, char move){
            // var sb = new StringBuilder();
            // sb.AppendLine($"{move} : {counter}");
            // foreach(var row in data.Filed) {
            //     sb.AppendLine(new string(row));
            // }
            // sb.AppendLine("================================================================================================================");
            // Debug.WriteLine(sb);
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            ########
                            #..O.O.#
                            ##@.O..#
                            #...O..#
                            #.#.O..#
                            #...O..#
                            #......#
                            ########

                            <^^>>>vv<v>>v<<
                            """,
                            1751
                        },            
            new object[]
                        {
                            """
                            ##########
                            #..O..O.O#
                            #......O.#
                            #.OO..O.O#
                            #..O@..O.#
                            #O#..O...#
                            #O..O..O.#
                            #.OO.O.OO#
                            #....O...#
                            ##########

                            <vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
                            vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
                            ><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
                            <<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
                            ^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
                            ^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
                            >^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
                            <><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
                            ^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
                            v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^
                            """,
                            9021
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day15.txt"),
                            1467145
                        },
    };
}
