using Xunit;
using FluentAssertions;
using System.Data;

namespace AoC2024;

public class Day16
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

        public static Direction[] Orthigonal(Direction direction) {
            if (Vertical.Contains(direction)) {
                return Horizontal;
            } else {
                return Vertical;
            }
        }

        public static Direction Opposite(Direction direction) {
            if (direction == Up) {
                return Down;
            }
            if (direction == Down) {
                return Up;
            }
            if (direction == Left) {
                return Right;
            }
            if (direction == Right) {
                return Left;
            }
            throw new ArgumentException();
        }
    }

    record Step(int I, int J, Direction Direction);
    record StepAndHistory(Step Step, List<int> History);

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var field = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.ToArray()).ToArray();
        var visited = new Dictionary<Step, int>();
        int n = field.Length;
        int m = field[0].Length;
        int iStart = 0;
        int jStart = 0;
        for (int k = 0; k < n * m; k++) {
            if (field[k / m][k % m] == 'S') {
                iStart = k / m;
                jStart = k % m;
            }
        }
        int ret = int.MaxValue;
        var q = new Queue<Step>();
        var startUp = new Step(iStart, jStart, Direction.Up);
        var startDown = new Step(iStart, jStart, Direction.Down);
        var startRight = new Step(iStart, jStart, Direction.Right);
        var startLeft = new Step(iStart, jStart, Direction.Left);
        q.Enqueue(startUp);
        q.Enqueue(startDown);
        q.Enqueue(startRight);
        q.Enqueue(startLeft);
        visited[startUp] = 1000;
        visited[startDown] = 1000;
        visited[startRight] = 0;
        visited[startLeft] = 2000;
        while(q.Any()) {
            var size = q.Count;
            while(size > 0) {
                var step = q.Dequeue();
                
                var cost = visited[step];
                var orthigonal = Direction.Orthigonal(step.Direction);
                while(field[step.I + step.Direction.I][step.J + step.Direction.J] != '#') {
                    cost++;
                    step = new Step(step.I + step.Direction.I, step.J + step.Direction.J, step.Direction);
                    if (visited.TryGetValue(step, out var existingCost)) {
                        if (existingCost < cost) {
                            break;
                        }
                    }
                    visited[step] = cost;
                    var stepOrt1 = step with {Direction = orthigonal[0]};
                    if (!visited.TryGetValue(stepOrt1, out var existingCostOrt1) || existingCostOrt1 > cost + 1000) {
                        q.Enqueue(stepOrt1);
                        visited[stepOrt1] = cost + 1000;
                    }
                    var stepOrt2 = step with {Direction = orthigonal[1]};
                    if (!visited.TryGetValue(stepOrt2, out var existingCostOrt2) || existingCostOrt2 > cost + 1000) {
                        q.Enqueue(stepOrt2);
                        visited[stepOrt2] = cost + 1000;
                    }
                    if (field[step.I][step.J] == 'E') {
                        ret = Math.Min(ret, cost);
                        break;
                    }
                }

                size--;
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
                            ###############
                            #.......#....E#
                            #.#.###.#.###.#
                            #.....#.#...#.#
                            #.###.#####.#.#
                            #.#.#.......#.#
                            #.#.#####.###.#
                            #...........#.#
                            ###.#.#####.#.#
                            #...#.....#.#.#
                            #.#.#.###.#.#.#
                            #.....#...#.#.#
                            #.###.#.#.#.#.#
                            #S..#.....#...#
                            ###############
                            """,
                            7036
                        },            
            new object[]
                        {
                            """
                            #################
                            #...#...#...#..E#
                            #.#.#.#.#.#.#.#.#
                            #.#.#.#...#...#.#
                            #.#.#.#.###.#.#.#
                            #...#.#.#.....#.#
                            #.#.#.#.#.#####.#
                            #.#...#.#.#.....#
                            #.#.#####.#.###.#
                            #.#.#.......#...#
                            #.#.###.#####.###
                            #.#.#...#.....#.#
                            #.#.#.#####.###.#
                            #.#.#.........#.#
                            #.#.#.#########.#
                            #S#.............#
                            #################
                            """,
                            11048
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day16.txt"),
                            91464
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var field = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.ToArray()).ToArray();
        var visited = new Dictionary<Step, int>();
        int n = field.Length;
        int m = field[0].Length;
        int iStart = 0;
        int jStart = 0;
        for (int k = 0; k < n * m; k++) {
            if (field[k / m][k % m] == 'S') {
                iStart = k / m;
                jStart = k % m;
            }
        }
        int bestCost = int.MaxValue;
        var q = new Queue<StepAndHistory>();
        var startUp = new Step(iStart, jStart, Direction.Up);
        var startDown = new Step(iStart, jStart, Direction.Down);
        var startRight = new Step(iStart, jStart, Direction.Right);
        var startLeft = new Step(iStart, jStart, Direction.Left);
        q.Enqueue(new StepAndHistory(startUp, new List<int>{iStart * m + jStart}));
        q.Enqueue(new StepAndHistory(startDown, new List<int>{iStart * m + jStart}));
        q.Enqueue(new StepAndHistory(startRight, new List<int>{iStart * m + jStart}));
        q.Enqueue(new StepAndHistory(startLeft, new List<int>{iStart * m + jStart}));
        visited[startUp] = 1000;
        visited[startDown] = 1000;
        visited[startRight] = 0;
        visited[startLeft] = 2000;
        var bestHistory = new List<StepAndHistory>();
        while(q.Any()) {
            var size = q.Count;
            while(size > 0) {
                var stepAndHistory = q.Dequeue();
                var step = stepAndHistory.Step;
                
                var cost = visited[step];
                var orthigonal = Direction.Orthigonal(step.Direction);
                while(field[step.I + step.Direction.I][step.J + step.Direction.J] != '#') {
                    cost++;
                    step = new Step(step.I + step.Direction.I, step.J + step.Direction.J, step.Direction);
                    if (visited.TryGetValue(step, out var existingCost)) {
                        if (existingCost < cost) {
                            break;
                        }
                    }
                    visited[step] = cost;
                    stepAndHistory.History.Add(step.I * m + step.J);
                    if (field[step.I][step.J] == 'E') {
                        if (bestCost > cost) {
                            bestHistory = new List<StepAndHistory>{stepAndHistory};
                            bestCost = cost;
                        } else {
                            if (bestCost == cost) {
                                bestHistory.Add(stepAndHistory);
                            }
                        }
                        break;
                    }
                    var stepOrt1 = step with {Direction = orthigonal[0]};
                    if (!visited.TryGetValue(stepOrt1, out var existingCostOrt1) || existingCostOrt1 >= cost + 1000) {
                        q.Enqueue(stepAndHistory with {Step = stepOrt1, History = stepAndHistory.History.ToList()});
                        visited[stepOrt1] = cost + 1000;
                    }
                    var stepOrt2 = step with {Direction = orthigonal[1]};
                    if (!visited.TryGetValue(stepOrt2, out var existingCostOrt2) || existingCostOrt2 >= cost + 1000) {
                        q.Enqueue(stepAndHistory with {Step = stepOrt2, History = stepAndHistory.History.ToList()});
                        visited[stepOrt2] = cost + 1000;
                    }
                }

                size--;
            }
        }

        var ret = bestHistory.SelectMany(x => x.History).Distinct().Count();
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
                        new object[]
                        {
                            """
                            ###############
                            #.......#....E#
                            #.#.###.#.###.#
                            #.....#.#...#.#
                            #.###.#####.#.#
                            #.#.#.......#.#
                            #.#.#####.###.#
                            #...........#.#
                            ###.#.#####.#.#
                            #...#.....#.#.#
                            #.#.#.###.#.#.#
                            #.....#...#.#.#
                            #.###.#.#.#.#.#
                            #S..#.....#...#
                            ###############
                            """,
                            45
                        },            
            new object[]
                        {
                            """
                            #################
                            #...#...#...#..E#
                            #.#.#.#.#.#.#.#.#
                            #.#.#.#...#...#.#
                            #.#.#.#.###.#.#.#
                            #...#.#.#.....#.#
                            #.#.#.#.#.#####.#
                            #.#...#.#.#.....#
                            #.#.#####.#.###.#
                            #.#.#.......#...#
                            #.#.###.#####.###
                            #.#.#...#.....#.#
                            #.#.#.#####.###.#
                            #.#.#.........#.#
                            #.#.#.#########.#
                            #S#.............#
                            #################
                            """,
                            64
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day16.txt"),
                            494
                        },
    };
}
