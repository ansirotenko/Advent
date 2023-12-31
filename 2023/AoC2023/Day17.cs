using Xunit;
using FluentAssertions;
namespace AoC2023;

public class Day17
{
    private int[][] Parse(string input)
    {
        return input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Select(d => d - '0').ToArray())
                .ToArray();
    }

    enum Direction { Top, Right, Bottom, Left }
    record Route(int I, int J, int Value, int Capacity, Direction Direction);

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var ret = Solve(input, 1, 2);
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            2413432311323
                            3215453535623
                            3255245654254
                            3446585845452
                            4546657867536
                            1438598798454
                            4457876987766
                            3637877979653
                            4654967986887
                            4564679986453
                            1224686865563
                            2546548887735
                            4322674655533
                            """,
                            102
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day17.txt"),
                            1065
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var ret = Solve(input, 4, 6);
        ret.Should().Be(expected);
    }


    private long Solve(string input, int leap, int afterLeap)
    {

        var data = Parse(input);
        var n = data.Length;
        var m = data[0].Length;
        var visited = Enum.GetValues<Direction>()
                .ToDictionary(
                    x => x,
                    _ => Enumerable.Range(0, afterLeap + 1)
                        .ToDictionary(
                            x => x,
                            _ => Enumerable.Range(0, n)
                                    .Select(_ => Enumerable.Range(0, m).Select(x => int.MaxValue).ToArray())
                                    .ToArray()
                            ));

        var q = new PriorityQueue<Route, int>();

        data[0][0] = 0;
        TryEnqueue(0, 0, 0, leap, 0, afterLeap, Direction.Right);
        TryEnqueue(0, leap, 0, 0, 0, afterLeap, Direction.Bottom);
        visited[Direction.Bottom][afterLeap][0][0] = 0;
        visited[Direction.Right][afterLeap][0][0] = 0;
        visited[Direction.Left][afterLeap][0][0] = 0;
        visited[Direction.Top][afterLeap][0][0] = 0;

        while (q.Count != 0)
        {
            var (i, j, v, c, d) = q.Dequeue();

            if (i == n - 1 && j == m - 1)
            {
                return v + data[i][j];
            }

            switch (d)
            {
                case Direction.Top:
                    TryEnqueue(i, i, j, j - leap, v, afterLeap, Direction.Left);
                    TryEnqueue(i, i, j, j + leap, v, afterLeap, Direction.Right);
                    TryEnqueue(i, i - 1, j, j, v, c - 1, Direction.Top);
                    break;
                case Direction.Bottom:
                    TryEnqueue(i, i, j, j - leap, v, afterLeap, Direction.Left);
                    TryEnqueue(i, i, j, j + leap, v, afterLeap, Direction.Right);
                    TryEnqueue(i, i + 1, j, j, v, c - 1, Direction.Bottom);
                    break;
                case Direction.Left:
                    TryEnqueue(i, i - leap, j, j, v, afterLeap, Direction.Top);
                    TryEnqueue(i, i + leap, j, j, v, afterLeap, Direction.Bottom);
                    TryEnqueue(i, i, j, j - 1, v, c - 1, Direction.Left);
                    break;
                case Direction.Right:
                    TryEnqueue(i, i - leap, j, j, v, afterLeap, Direction.Top);
                    TryEnqueue(i, i + leap, j, j, v, afterLeap, Direction.Bottom);
                    TryEnqueue(i, i, j, j + 1, v, c - 1, Direction.Right);
                    break;
            }
        }

        void TryEnqueue(int iStart, int i, int jStart, int j, int value, int capacity, Direction direction)
        {
            if (capacity >= 0 && i >= 0 && i < n && j >= 0 && j < m)
            {
                while (iStart != i)
                {
                    value += data![iStart][j];
                    if (iStart > i)
                        iStart--;
                    else
                        iStart++;
                }

                while (jStart != j)
                {
                    value += data![i][jStart];
                    if (jStart > j)
                        jStart--;
                    else
                        jStart++;
                }

                var hasBetter = false;
                for (var k = capacity; k <= afterLeap; k++)
                {
                    if (visited[direction][k][i][j] <= value)
                    {
                        hasBetter = true;
                        break;
                    }
                }

                if (!hasBetter)
                {
                    visited[direction][capacity][i][j] = value;
                    q!.Enqueue(new Route(i, j, value, capacity, direction), value);
                }
            }
        }

        return -1;
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
       {
            new object[]
                        {
                            """
                            2413432311323
                            3215453535623
                            3255245654254
                            3446585845452
                            4546657867536
                            1438598798454
                            4457876987766
                            3637877979653
                            4654967986887
                            4564679986453
                            1224686865563
                            2546548887735
                            4322674655533
                            """,
                            94
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day17.txt"),
                            1249
                        },
       };
}
