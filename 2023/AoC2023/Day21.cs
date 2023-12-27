using Xunit;
using FluentAssertions;
namespace AoC2023;

public class Day21
{
    private char[][] Parse(string input)
    {
        return input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.ToArray())
            .ToArray();
    }

    private (int, int)[] indexDirections = [(0, 1), (0, -1), (1, 0), (-1, 0)];

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int steps, long expected)
    {
        var data = Parse(input);
        long ret = Solve(data, steps);

        ret.Should().Be(expected);
    }

    private long Solve(char[][] data, int steps)
    {
        var n = data.Length;
        var visited = new Dictionary<(int, int), int>();

        var q = new Queue<(int, int)>();

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (data[i][j] == 'S')
                {
                    q.Enqueue((i, j));
                    break;
                }
            }
            if (q.Any())
                break;
        }

        visited.Add(q.First(), -1);

        while (steps >=0 )
        {
            var size = q.Count;
            while (size > 0)
            {
                var (i, j) = q.Dequeue();
                var ni = i % n;
                if (ni < 0) {
                    ni += n;
                }
                var nj = j % n;
                if (nj < 0) {
                    nj += n;
                }
                if (data[ni][nj] != '#' && (!visited.TryGetValue((i, j), out var r) || r < steps))
                {
                    visited[(i, j)] = steps;

                    foreach (var (di, dj) in indexDirections)
                    {
                        q.Enqueue((i + di, j + dj));
                    }
                }

                size--;
            }
            steps--;
        }

        return visited.Values.Count(x => x % 2 == 0);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            ...........
                            .....###.#.
                            .###.##..#.
                            ..#.#...#..
                            ....#.#....
                            .##..S####.
                            .##..#...#.
                            .......##..
                            .##.#.####.
                            .##..##.##.
                            ...........
                            """,
                            6,
                            16
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day21.txt"),
                            64,
                            3578
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int steps, long expected)
    {
        var data = Parse(input);
        var n = data.Length;
        var halfN = data.Length / 2;
        var s1 = Solve(data, halfN);
        var s2 = Solve(data, halfN + n);
        var s3 = Solve(data, halfN + n * 2);
        var a = s1 / 2 - s2 + s3 / 2;
        var b = -3 * (s1 / 2) + 2 * s2 - s3 / 2;
        var c = s1;
        var iterations = (steps - halfN) / n;
        var ret = a * iterations * iterations + b * iterations + c;
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
        {

            new object[]
                        {
                            File.ReadAllText("Inputs/Day21.txt"),
                            26501365,
                            594115391548176
                        },
       };
}
