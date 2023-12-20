using Xunit;
using FluentAssertions;
namespace AoC2023;

public class Day10
{
    private string[] Parse(string input)
    {
        return input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
    }

    record Point(int I, int J);

    private Dictionary<int, Dictionary<char, int>> fromToDirections = new Dictionary<int, Dictionary<char, int>>()
    {
        {0, new Dictionary<char, int>() {{'F', 1}, {'7', 3}, {'|', 0}}}, // top
        {1, new Dictionary<char, int>() {{'J', 0}, {'7', 2}, {'-', 1}}}, // right
        {2, new Dictionary<char, int>() {{'J', 3}, {'L', 1}, {'|', 2}}}, // bottom
        {3, new Dictionary<char, int>() {{'F', 2}, {'L', 0}, {'-', 3}}}, // left
    };

    private Dictionary<int, Dictionary<char, Point[]>> leftTraversal = new Dictionary<int, Dictionary<char, Point[]>>()
    {
        {0, new Dictionary<char, Point[]>() {{'F', new []{new Point(-1, 0), new Point(0, -1)}}, {'7', new Point[0]}, {'|', new []{new Point(0, -1)}}}}, // top
        {1, new Dictionary<char, Point[]>() {{'J', new Point[0]}, {'7',  new []{new Point(-1, 0), new Point(0, 1)}}, {'-', new []{new Point(-1, 0)}}}}, // right
        {2, new Dictionary<char, Point[]>() {{'J', new []{new Point(0, 1), new Point(1, 0)}}, {'L', new Point[0]}, {'|', new []{new Point(0, 1)}}}}, // bottom
        {3, new Dictionary<char, Point[]>() {{'F', new Point[0]}, {'L', new []{new Point(1, 0), new Point(0, -1)}}, {'-', new []{new Point(1, 0)}}}}, // left
    };

    private Point[] indexDirections = new[] { new Point(0, 1), new Point(0, -1), new Point(1, 0), new Point(-1, 0) };

    private Point NextTile(int i, int j, int toDirection)
    {
        return toDirection switch
        {
            0 => new Point(i - 1, j),
            1 => new Point(i, j + 1),
            2 => new Point(i + 1, j),
            3 => new Point(i, j - 1),
            _ => throw new Exception("Worng direction")
        };
    }

    private bool CanPipe(int i, int j, int fromDirection, string[] data)
    {
        return i >= 0 && i < data.Length &&
               j >= 0 && j < data[i].Length &&
               (fromToDirections[fromDirection].Keys.Contains(data[i][j]) || data[i][j] == 'S');
    }

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = Parse(input);
        var routes = new List<(int i, int j, int fromDirection)>();

        for (int i = 0; i < data.Length; i++)
        {
            for (int j = 0; j < data[i].Length; j++)
            {
                if (data[i][j] == 'S')
                {
                    for (int d = 0; d < 4; d++)
                    {
                        var (ni, nj) = NextTile(i, j, d);
                        if (CanPipe(ni, nj, d, data))
                            routes.Add((ni, nj, d));
                    }
                    break;
                }
            }
        }

        var ret = 0L;
        var iteration = 1;
        while (routes.Count > 1)
        {
            iteration++;
            var toBeDeleted = new List<int>();
            for (int k = 0; k < routes.Count; k++)
            {
                var route = routes[k];
                var toDirection = fromToDirections[route.fromDirection][data[route.i][route.j]];
                var (ni, nj) = NextTile(route.i, route.j, toDirection);
                if (!CanPipe(ni, nj, toDirection, data))
                {
                    toBeDeleted.Add(k);
                }
                else
                {
                    routes[k] = (ni, nj, toDirection);
                    for (int p = 0; p < routes.Count; p++)
                    {
                        if (p != k && routes[p].i == routes[k].i && routes[p].j == routes[k].j)
                        {
                            toBeDeleted.Add(p);
                            toBeDeleted.Add(k);
                            ret = p > k ? iteration - 1 : iteration;
                            break;
                        }
                    }
                }
            }

            foreach (var k in toBeDeleted.Distinct().OrderByDescending(x => x))
            {
                routes.RemoveAt(k);
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
                            .....
                            .S-7.
                            .|.|.
                            .L-J.
                            .....
                            """,
                            4
                        },
            new object[]
                        {
                            """
                            ..F7.
                            .FJ|.
                            SJ.L7
                            |F--J
                            LJ...
                            """,
                            8
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day10.txt"),
                            6897
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = Parse(input);
        var routes = new List<(int i, int j, int fromDirection)>();
        var projection = Enumerable.Range(0, data.Length)
                        .Select(i => Enumerable.Range(0, data[i].Length).Select(j => '.').ToArray())
                        .ToArray();

        var hasLoop = false;
        var inverse = false;
        for (int ii = 0; ii < data.Length; ii++)
        {
            for (int jj = 0; jj < data[ii].Length; jj++)
            {
                if (data[ii][jj] == 'S')
                {
                    for (int dd = 0; dd < 4; dd++)
                    {
                        var (i, j) = NextTile(ii, jj, dd);
                        var d = dd;
                        if (CanPipe(i, j, dd, data))
                        {
                            inverse = false;
                            projection[ii][jj] = '#';
                            while (true)
                            {
                                MarkProjectionByPipe(i, j, d, data[i][j], projection);

                                var toDirection = fromToDirections[d][data[i][j]];
                                var (ni, nj) = NextTile(i, j, toDirection);
                                if (!CanPipe(ni, nj, toDirection, data))
                                {
                                    hasLoop = false;
                                    break;
                                }
                                else
                                {
                                    if (data[ni][nj] == 'S')
                                    {
                                        hasLoop = true;
                                        break;
                                    }

                                    i = ni;
                                    j = nj;
                                    d = toDirection;
                                }
                            }

                        }

                        if (hasLoop)
                        {
                            CompleteProjection(projection);
                            break;
                        }
                        else
                        {
                            ClearProjection(projection);
                        }
                    }
                    break;
                }
            }
        }

        var retL = 0L;
        var retR = 0L;
        for (int i = 0; i < projection.Length; i++)
        {
            for (int j = 0; j < projection[i].Length; j++)
            {
                if (projection[i][j] == 'X')
                {
                    retL++;
                }
                else
                {
                    if (projection[i][j] == '.')
                    {
                        retR++;
                    }
                }
            }
        }

        var ret = inverse ? retR : retL;
        ret.Should().Be(expected);

        void CompleteProjection(char[][] projection)
        {
            for (int ii = 0; ii < projection.Length; ii++)
            {
                for (int jj = 0; jj < projection[ii].Length; jj++)
                {
                    if (projection[ii][jj] == 'I')
                    {
                        var q = new Queue<Point>();
                        q.Enqueue(new Point(ii, jj));
                        projection[ii][jj] = 'X';

                        while (q.Any())
                        {
                            var (i, j) = q.Dequeue();
                            
                            foreach (var (di, dj) in indexDirections)
                            {
                                var ni = di + i;
                                var nj = dj + j;

                                if (ni >= 0 && ni < projection.Length && nj >= 0 && nj < projection[ni].Length)
                                {
                                    if (projection[ni][nj] == 'I' || projection[ni][nj] == '.')
                                    {
                                        projection[ni][nj] = 'X';
                                        q.Enqueue(new Point(ni, nj));
                                    }
                                }
                                else
                                {
                                    inverse = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        void MarkProjectionByPipe(int i, int j, int d, char c, char[][] projection)
        {
            projection[i][j] = '#';
            var tomark = leftTraversal[d][c];
            foreach (var (di, dj) in tomark)
            {
                var ni = i + di;
                var nj = j + dj;
                if (ni >= 0 && ni < projection.Length && nj >= 0 && nj < projection[ni].Length)
                {
                    if (projection[ni][nj] == '.')
                    {
                        projection[ni][nj] = 'I';
                    }
                }
                else
                {
                    inverse = true;
                }
            }
        }

        void ClearProjection(char[][] projection)
        {
            for (int i = 0; i < projection.Length; i++)
            {
                for (int j = 0; j < projection[i].Length; j++)
                {
                    projection[i][j] = '.';
                }
            }
        }
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
        {
            new object[]
                        {
                            """
                            ...........
                            .S-------7.
                            .|F-----7|.
                            .||.....||.
                            .||.....||.
                            .|L-7.F-J|.
                            .|..|.|..|.
                            .L--J.L--J.
                            ...........
                            """,
                            4
                        },
            new object[]
                        {
                            """
                            ..........
                            .S------7.
                            .|F----7|.
                            .||....||.
                            .||....||.
                            .|L-7F-J|.
                            .|..||..|.
                            .L--JL--J.
                            ..........
                            """,
                            4
                        },
            new object[]
                        {
                            """
                            .F----7F7F7F7F-7....
                            .|F--7||||||||FJ....
                            .||.FJ||||||||L7....
                            FJL7L7LJLJ||LJ.L-7..
                            L--J.L7...LJS7F-7L7.
                            ....F-J..F7FJ|L7L7L7
                            ....L7.F7||L7|.L7L7|
                            .....|FJLJ|FJ|F7|.LJ
                            ....FJL-7.||.||||...
                            ....L---J.LJ.LJLJ...
                            """,
                            8
                        },
            new object[]
                        {
                            """
                            FF7FSF7F7F7F7F7F---7
                            L|LJ||||||||||||F--J
                            FL-7LJLJ||||||LJL-77
                            F--JF--7||LJLJ7F7FJ-
                            L---JF-JLJ.||-FJLJJ7
                            |F|F-JF---7F7-L7L|7|
                            |FFJF7L7F-JF7|JL---7
                            7-L-JL7||F7|L7F-7F7|
                            L.L7LFJ|||||FJL7||LJ
                            L7JLJL-JLJLJL--JLJ.L
                            """,
                            10
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day10.txt"),
                            367
                        },
       };
}
