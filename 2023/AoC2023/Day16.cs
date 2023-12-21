using Xunit;
using FluentAssertions;
namespace AoC2023;

public class Day16
{
    private string[] Parse(string input)
    {
        return input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
    }

    [Flags]
    enum Direction { None = 0, Top = 1, Right = 2, Bottom = 4, Left = 8 }
    record Data(int I, int J, Direction Direction);

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = Parse(input);
        var n = data.Length;
        var m = data[0].Length;
        long ret = GetResult(data, n, m, new Data(0, 0, Direction.Right));
        ret.Should().Be(expected);
    }

    long GetResult(string[] data, int n, int m, Data start)
    {
        var energy = Enumerable.Range(0, n)
                        .Select(_ => Enumerable.Range(0, m).Select(_ => Direction.None).ToArray())
                        .ToArray();
        var q = new Queue<Data>();
        q.Enqueue(start);

        while (q.Any())
        {
            int size = q.Count;
            while (size > 0)
            {
                size--;
                var (i, j, d) = q.Dequeue();
                if (i >= 0 && i < n && j >= 0 && j < m && (energy[i][j] & d) == 0)
                {
                    energy[i][j] = energy[i][j] | d;
                    switch (d)
                    {
                        case Direction.Top:
                            switch (data[i][j])
                            {
                                case '.':
                                case '|':
                                    q.Enqueue(new Data(i - 1, j, Direction.Top));
                                    break;
                                case '\\':
                                    q.Enqueue(new Data(i, j - 1, Direction.Left));
                                    break;
                                case '/':
                                    q.Enqueue(new Data(i, j + 1, Direction.Right));
                                    break;
                                case '-':
                                    q.Enqueue(new Data(i, j + 1, Direction.Right));
                                    q.Enqueue(new Data(i, j - 1, Direction.Left));
                                    break;
                            }

                            break;
                        case Direction.Right:
                            switch (data[i][j])
                            {
                                case '.':
                                case '-':
                                    q.Enqueue(new Data(i, j + 1, Direction.Right));
                                    break;
                                case '\\':
                                    q.Enqueue(new Data(i + 1, j, Direction.Bottom));
                                    break;
                                case '/':
                                    q.Enqueue(new Data(i - 1, j, Direction.Top));
                                    break;
                                case '|':
                                    q.Enqueue(new Data(i - 1, j, Direction.Top));
                                    q.Enqueue(new Data(i + 1, j, Direction.Bottom));
                                    break;
                            }

                            break;
                        case Direction.Bottom:
                            switch (data[i][j])
                            {
                                case '.':
                                case '|':
                                    q.Enqueue(new Data(i + 1, j, Direction.Bottom));
                                    break;
                                case '\\':
                                    q.Enqueue(new Data(i, j + 1, Direction.Right));
                                    break;
                                case '/':
                                    q.Enqueue(new Data(i, j - 1, Direction.Left));
                                    break;
                                case '-':
                                    q.Enqueue(new Data(i, j + 1, Direction.Right));
                                    q.Enqueue(new Data(i, j - 1, Direction.Left));
                                    break;
                            }
                            break;
                        case Direction.Left:
                            switch (data[i][j])
                            {
                                case '.':
                                case '-':
                                    q.Enqueue(new Data(i, j - 1, Direction.Left));
                                    break;
                                case '\\':
                                    q.Enqueue(new Data(i - 1, j, Direction.Top));
                                    break;
                                case '/':
                                    q.Enqueue(new Data(i + 1, j, Direction.Bottom));
                                    break;
                                case '|':
                                    q.Enqueue(new Data(i - 1, j, Direction.Top));
                                    q.Enqueue(new Data(i + 1, j, Direction.Bottom));
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        return energy.Sum(r => r.Count(x => x > 0));
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            .|...\....
                            |.-.\.....
                            .....|-...
                            ........|.
                            ..........
                            .........\
                            ..../.\\..
                            .-.-/..|..
                            .|....-|.\
                            ..//.|....
                            """,
                            46
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day16.txt"),
                            7884
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = Parse(input);
        var n = data.Length;
        var m = data[0].Length;

        var ret = 0L;

        for (int i = 0; i < n; i++)
        {
            ret = Math.Max(ret, GetResult(data, n, m, new Data(i, 0, Direction.Right)));
            ret = Math.Max(ret, GetResult(data, n, m, new Data(i, m - 1, Direction.Left)));
        }

        for (int j = 0; j < m; j++)
        {
            ret = Math.Max(ret, GetResult(data, n, m, new Data(0, j, Direction.Bottom)));
            ret = Math.Max(ret, GetResult(data, n, m, new Data(n-1, j, Direction.Top)));
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
        {
            new object[]
                        {
                            """
                            .|...\....
                            |.-.\.....
                            .....|-...
                            ........|.
                            ..........
                            .........\
                            ..../.\\..
                            .-.-/..|..
                            .|....-|.\
                            ..//.|....
                            """,
                            51
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day16.txt"),
                            8185
                        },
       };
}
