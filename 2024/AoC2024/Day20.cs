using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day20
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

        public static Direction[] Next(Direction direction) {
            if (direction == Up) {
                return new []{Down, Left, Right};
            }
            if (direction == Down) {
                return new []{Up, Left, Right};
            }
            if (direction == Left) {
                return new []{Right, Up, Down};
            }
            if (direction == Right) {
                return new []{Left, Up, Down};
            }
            throw new ArgumentException();
        }
    }

    record Point(int I, int J);

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int economy, int expected)
    {
        var field = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.ToArray())
            .ToArray();
        int n = field.Length;
        int m = field[0].Length;
        Point? start = null;
        Point? end = null;
        int emptySpaces = 0;
        for(int k = 0; k < n * m; k++) {
            switch(field[k / m][k % m]) {
                case 'S':
                    start = new Point(k / m, k % m);
                    break;
                case 'E':
                    end = new Point(k / m, k % m);
                    break;
                case '.':
                    emptySpaces++;
                    break;
            }
        }
        if (end == null || start == null) {
            throw new ArgumentException();
        }

        var route = new Dictionary<Point, int>{
            {start, 0}
        };
        Point current = start;
        int iteration = 0;
        while (field[current.I][current.J] != 'E') {
            iteration++;
            foreach(var direction in Direction.All) {
                var next = new Point(current.I + direction.I, current.J + direction.J);
                if (field[next.I][next.J] != '#' && !route.ContainsKey(next)) {
                    route[next] = iteration;
                    current = next;
                    break;
                }
            }
        }

        var savedRoutes = new Dictionary<int, int>();
        foreach (var routePoint in route.Keys) {
            foreach(var direction in Direction.All) {
                var next = new Point(routePoint.I + direction.I, routePoint.J + direction.J);
                if (field[next.I][next.J] == '#') {
                    var nextNext = new Point(next.I + direction.I, next.J + direction.J);
                    if (nextNext.I >= 0 && nextNext.I < n && nextNext.J >= 0 && nextNext.J < m && route.ContainsKey(nextNext)) {
                        var save = route[nextNext] - route[routePoint] - 2;
                        if (save >= economy) {
                            if (!savedRoutes.ContainsKey(save)) {
                                savedRoutes[save] = 0;
                            }
                            savedRoutes[save]++;
                        }
                    }
                }
            }
        }

        var ret = savedRoutes.Values.Sum();
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            ###############
                            #...#...#.....#
                            #.#.#.#.#.###.#
                            #S#...#.#.#...#
                            #######.#.#.###
                            #######.#.#...#
                            #######.#.###.#
                            ###..E#...#...#
                            ###.#######.###
                            #...###...#...#
                            #.#####.#.###.#
                            #.#...#.#.#...#
                            #.#.#.#.#.#.###
                            #...#...#...###
                            ###############
                            """,
                            0,
                            44
                       },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day20.txt"),
                            100,
                            1409
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int economy, int expected)
    {
        var nCheats = 20;
        var field = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.ToArray())
            .ToArray();
        int n = field.Length;
        int m = field[0].Length;
        Point? start = null;
        Point? end = null;
        int emptySpaces = 0;
        for(int k = 0; k < n * m; k++) {
            switch(field[k / m][k % m]) {
                case 'S':
                    start = new Point(k / m, k % m);
                    break;
                case 'E':
                    end = new Point(k / m, k % m);
                    break;
                case '.':
                    emptySpaces++;
                    break;
            }
        }
        if (end == null || start == null) {
            throw new ArgumentException();
        }

        var route = new Dictionary<Point, int>{
            {start, 0}
        };
        Point current = start;
        int iteration = 0;
        while (field[current.I][current.J] != 'E') {
            iteration++;
            foreach(var direction in Direction.All) {
                var next = new Point(current.I + direction.I, current.J + direction.J);
                if (field[next.I][next.J] != '#' && !route.ContainsKey(next)) {
                    route[next] = iteration;
                    current = next;
                    break;
                }
            }
        }

        var savedRoutes = new Dictionary<int, int>();
        foreach (var routePoint in route.Keys) {
            var cheats = 0;
            var visited = new HashSet<Point>();
            var cheatDestination = new HashSet<Point>();
            var q = new Queue<Point>();
            q.Enqueue(routePoint);
            visited.Add(routePoint);
            while(cheats < nCheats) {
                var size = q.Count;
                while(size > 0) {
                    var curr = q.Dequeue();
                    foreach(var direction in Direction.All) {
                        var next = new Point(curr.I + direction.I, curr.J + direction.J);
                        if (next.I >= 0 && next.I < n && next.J >= 0 && next.J < m && !visited.Contains(next)) {
                            q.Enqueue(next);
                            visited.Add(next);
                            if (!cheatDestination.Contains(next) && route.ContainsKey(next)) {
                                cheatDestination.Add(next);
                                var save = route[next] - route[routePoint] - cheats;
                                if (save >= economy) {
                                    if (!savedRoutes.ContainsKey(save)) {
                                        savedRoutes[save] = 0;
                                    }
                                    savedRoutes[save]++;
                                }
                            }
                        }
                    }
                    size--;
                }
                cheats++;
            }
        }

        var ret = savedRoutes.Values.Sum();
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            ###############
                            #...#...#.....#
                            #.#.#.#.#.###.#
                            #S#...#.#.#...#
                            #######.#.#.###
                            #######.#.#...#
                            #######.#.###.#
                            ###..E#...#...#
                            ###.#######.###
                            #...###...#...#
                            #.#####.#.###.#
                            #.#...#.#.#...#
                            #.#.#.#.#.#.###
                            #...#...#...###
                            ###############
                            """,
                            50,
                            285
                       },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day20.txt"),
                            100,
                            1012821
                        },
    };
}
