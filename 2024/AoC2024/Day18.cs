using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day18
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
        public static Direction[] Horizontal = new [] {Left, Right};
        public static Direction[] Vertical = new [] {Up, Down};
    }

    record Point(int I, int J);

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int mapSize, int limit, int expected)
    {
        var allBytes = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => {
                    var split = x.Split(new []{","}, StringSplitOptions.RemoveEmptyEntries);
                    return new Point(int.Parse(split[1]), int.Parse(split[0]));
                })
                .ToArray();
        
        var ret = Solve(limit);

        int Solve(int limit) {
            var bytes = allBytes.Take(limit).ToHashSet();
            var visited = new Dictionary<Point, int>();
            var q = new Queue<Point>();
            q.Enqueue(new Point(0, 0));
            visited[new Point(0, 0)] = 0;
            int ret = 0;
            while (q.Any()) {
                var size = q.Count;
                ret++;
                while(size > 0) {
                    var next = q.Dequeue();
                    foreach(var direction in Direction.All) {
                        var current = new Point(next.I + direction.I, next.J + direction.J);
                        if (current.I >= 0 && current.I < mapSize && 
                            current.J >= 0 && current.J < mapSize && 
                            !bytes.Contains(current) &&
                            (!visited.TryGetValue(current, out var existed) || existed > ret)) {
                            if (current.I == mapSize - 1 && current.J == mapSize - 1) {
                                return ret;
                            }
                            visited[current] = ret;
                            q.Enqueue(current);
                        }
                    }
                    size--;
                }
            }

            return -1;
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            5,4
                            4,2
                            4,5
                            3,0
                            2,1
                            6,3
                            2,4
                            1,5
                            0,6
                            3,3
                            2,6
                            5,1
                            1,2
                            5,5
                            2,5
                            6,5
                            1,4
                            0,4
                            6,4
                            1,1
                            6,1
                            1,0
                            0,5
                            1,6
                            2,0
                            """,
                            7,
                            12,
                            22
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day18.txt"),
                            71,
                            1024,
                            294
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int mapSize, int limit, string expected)
    {
        var allBytes = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => {
                    var split = x.Split(new []{","}, StringSplitOptions.RemoveEmptyEntries);
                    return new Point(int.Parse(split[1]), int.Parse(split[0]));
                })
                .ToArray();
        
        while (true) {
            if (Solve(limit) == -1) {
                break;
            }
            limit++;
        }

        var ret = $"{allBytes[limit - 1].J},{allBytes[limit - 1].I}";

        int Solve(int limit) {
            var bytes = allBytes.Take(limit).ToHashSet();
            var visited = new Dictionary<Point, int>();
            var q = new Queue<Point>();
            q.Enqueue(new Point(0, 0));
            visited[new Point(0, 0)] = 0;
            int ret = 0;
            while (q.Any()) {
                var size = q.Count;
                ret++;
                while(size > 0) {
                    var next = q.Dequeue();
                    foreach(var direction in Direction.All) {
                        var current = new Point(next.I + direction.I, next.J + direction.J);
                        if (current.I >= 0 && current.I < mapSize && 
                            current.J >= 0 && current.J < mapSize && 
                            !bytes.Contains(current) &&
                            (!visited.TryGetValue(current, out var existed) || existed > ret)) {
                            if (current.I == mapSize - 1 && current.J == mapSize - 1) {
                                return ret;
                            }
                            visited[current] = ret;
                            q.Enqueue(current);
                        }
                    }
                    size--;
                }
            }

            return -1;
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
           new object[]
                        {
                            """
                            5,4
                            4,2
                            4,5
                            3,0
                            2,1
                            6,3
                            2,4
                            1,5
                            0,6
                            3,3
                            2,6
                            5,1
                            1,2
                            5,5
                            2,5
                            6,5
                            1,4
                            0,4
                            6,4
                            1,1
                            6,1
                            1,0
                            0,5
                            1,6
                            2,0
                            """,
                            7,
                            12,
                            "6,1"
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day18.txt"),
                            71,
                            1024,
                            "31,22"
                        },
    };
}
