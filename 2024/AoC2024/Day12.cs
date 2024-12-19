using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day12
{
    private static (int di, int dj)[] directions = new [] {
        (1, 0), // down
        (0, -1), // left
        (-1, 0), //up
        (0, 1), // right
    };

    struct Point {
        public Point() {}
        public Point(int i, int j) {
            I = i;
            J = j;
        }

        public int I { get; set; }
        public int J { get; set; }
    };

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var rows = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        var squares = rows.Select(r => new int[r.Length]).ToArray();

        for (int i = 0; i < rows.Length; i++) {
            for (int j = 0; j < rows[i].Length; j++) {
                if (squares[i][j] == 0) {
                    var square = 0;
                    var visited = new HashSet<Point>();
                    var q = new Stack<Point>();
                    var startPoint = new Point(i, j);
                    q.Push(startPoint);
                    visited.Add(startPoint);
                    while(q.Any()) {
                        var current = q.Pop();
                        square++;
                        foreach(var direction in directions) {
                            var next = new Point(current.I + direction.di, current.J + direction.dj);
                            if (GetChar(next) != ' ' && 
                                GetChar(next) == GetChar(startPoint) &&
                                !visited.Contains(next)) {
                                visited.Add(next);
                                q.Push(next);
                            }
                        }
                    }
                    foreach(var p in visited) {
                        squares[p.I][p.J] = square;
                    }
                }
            }
        }

        var ret = 0;
        for (int i = 0; i < rows.Length; i++) {
            for (int j = 0; j < rows[i].Length; j++) {
                
                if (j == rows[i].Length - 1) {
                    ret += squares[i][j];
                } else {
                    if (j == 0) {
                        ret += squares[i][j];
                    }
                    if (rows[i][j] != rows[i][j+1]) {
                        ret += squares[i][j];
                        ret += squares[i][j+1];
                    }
                }

                if (j == rows.Length - 1) {
                    ret += squares[j][i];
                } else {
                    if (j == 0) {
                        ret += squares[j][i];
                    }
                    if (rows[j][i] != rows[j+1][i]) {
                        ret += squares[j][i];
                        ret += squares[j+1][i];
                    }
                }
            }
        }

        char GetChar(Point p) {
            if (p.I >= 0 && p.I < rows.Length && p.J >= 0 && p.J < rows[p.I].Length) {
                return rows[p.I][p.J];
            }
            return ' ';
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            AAAA
                            BBCD
                            BBCC
                            EEEC
                            """,
                            140
                        },
            new object[]
                        {
                            """
                            OOOOO
                            OXOXO
                            OOOOO
                            OXOXO
                            OOOOO
                            """,
                            772
                        },            
            new object[]
                        {
                            """
                            RRRRIICCFF
                            RRRRIICCCF
                            VVRRRCCFFF
                            VVRCCCJFFF
                            VVVVCJJCFE
                            VVIVCCJJEE
                            VVIIICJJEE
                            MIIIIIJJEE
                            MIIISIJEEE
                            MMMISSJEEE
                            """,
                            1930
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day12.txt"),
                            1319878
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var rows = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        var squares = rows.Select(r => new int[r.Length]).ToArray();

        for (int i = 0; i < rows.Length; i++) {
            for (int j = 0; j < rows[i].Length; j++) {
                if (squares[i][j] == 0) {
                    var square = 0;
                    var visited = new HashSet<Point>();
                    var q = new Stack<Point>();
                    var startPoint = new Point(i, j);
                    q.Push(startPoint);
                    visited.Add(startPoint);
                    while(q.Any()) {
                        var current = q.Pop();
                        square++;
                        foreach(var direction in directions) {
                            var next = new Point(current.I + direction.di, current.J + direction.dj);
                            if (GetChar(next) != ' ' && 
                                GetChar(next) == GetChar(startPoint) &&
                                !visited.Contains(next)) {
                                visited.Add(next);
                                q.Push(next);
                            }
                        }
                    }
                    foreach(var p in visited) {
                        squares[p.I][p.J] = square;
                    }
                }
            }
        }

        var ret = 0;
        for (int i = -1; i < rows.Length; i++) {
            var top = ' ';
            var bottom = ' ';
            var left = ' ';
            var right = ' ';
                
            for (int j = 0; j < rows[0].Length; j++) {
                var topPoint = new Point(i, j);
                var bottomPoint = new Point(i+1, j);
                var leftPoint = new Point(j, i);
                var rightPoint = new Point(j, i+1);
                
                if (GetChar(topPoint) != GetChar(bottomPoint)) {
                    if (top != bottom) {
                        if (GetChar(topPoint) != top) {
                            ret += GetSquare(topPoint);
                        }
                        if (GetChar(bottomPoint) != bottom) {
                            ret += GetSquare(bottomPoint);
                        }
                    } else {
                        ret += GetSquare(topPoint);
                        ret += GetSquare(bottomPoint);
                    }
                }
                top = GetChar(topPoint);
                bottom = GetChar(bottomPoint);

                if (GetChar(leftPoint) != GetChar(rightPoint)) {
                    if (left != right) {
                        if (GetChar(leftPoint) != left) {
                            ret += GetSquare(leftPoint);
                        }
                        if (GetChar(rightPoint) != right) {
                            ret += GetSquare(rightPoint);
                        }
                    } else {
                        ret += GetSquare(leftPoint);
                        ret += GetSquare(rightPoint);
                    }
                }
                
                left = GetChar(leftPoint);
                right = GetChar(rightPoint);
            }
        }

        char GetChar(Point p) {
            if (p.I >= 0 && p.I < rows.Length && p.J >= 0 && p.J < rows[p.I].Length) {
                return rows[p.I][p.J];
            }
            return ' ';
        }
        int GetSquare(Point p) {
            if (p.I >= 0 && p.I < rows.Length && p.J >= 0 && p.J < rows[p.I].Length) {
                return squares[p.I][p.J];
            }
            return 0;
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
                       new object[]
                        {
                            """
                            AAAA
                            BBCD
                            BBCC
                            EEEC
                            """,
                            80
                        },
            new object[]
                        {
                            """
                            OOOOO
                            OXOXO
                            OOOOO
                            OXOXO
                            OOOOO
                            """,
                            436
                        },    
            new object[]
                        {
                            """
                            EEEEE
                            EXXXX
                            EEEEE
                            EXXXX
                            EEEEE
                            """,
                            236
                        },  
            new object[]
                        {
                            """
                            AAAAAA
                            AAABBA
                            AAABBA
                            ABBAAA
                            ABBAAA
                            AAAAAA
                            """,
                            368
                        },          
            new object[]
                        {
                            """
                            RRRRIICCFF
                            RRRRIICCCF
                            VVRRRCCFFF
                            VVRCCCJFFF
                            VVVVCJJCFE
                            VVIVCCJJEE
                            VVIIICJJEE
                            MIIIIIJJEE
                            MIIISIJEEE
                            MMMISSJEEE
                            """,
                            1206
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day12.txt"),
                            784982
                        },
    };
}
