using Xunit;
using FluentAssertions;
namespace AoC2023;

public class Day14
{
    private char[][] Parse(string input)
    {
        return input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToArray())
                .ToArray();
    }

    private string Format(char[][] data)
    {
        return string.Join("\n", data.Select(x => new string(x)));
    }

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = Parse(input);
        Rotate(data);
        Tilt(data);
        long ret = GetResult(data);

        ret.Should().Be(expected);
    }

    void Rotate(char[][] data)
    {
        var n = data.Length;
        char tmp;
        for (int i = 0; i < n; i++)
        {
            for (int j = i; j < n; j++)
            {
                tmp = data[i][j];
                data[i][j] = data[j][i];
                data[j][i] = tmp;
            }
        }
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n / 2; j++)
            {
                tmp = data[i][j];
                data[i][j] = data[i][n - 1 - j];
                data[i][n - 1 - j] = tmp;
            }
        }
    }

    void Tilt(char[][] data)
    {
        var n = data.Length;
        for (int i = 0; i < n; i++)
        {
            int j = n - 1;
            while (j >= 0)
            {
                while (j >= 0 && data[i][j] == 'O')
                {
                    j--;
                }

                int start = j;
                while (j >= 0 && data[i][j] != '#')
                {
                    if (data[i][j] == 'O')
                    {
                        data[i][j] = data[i][start];
                        data[i][start] = 'O';
                        start--;
                    }
                    else
                    {
                        j--;
                    }
                }

                while (j >= 0 && data[i][j] == '#')
                {
                    j--;
                }
            }
        }
    }

    long GetResult(char[][] data)
    {
        var n = data.Length;
        var ret = 0L;
        for (int i = 0; i < n; i++)
        {
            int j = n-1;
            while (j >= 0)
            {
                int start = j;
                while (j >= 0 && data[i][j] == 'O')
                {
                    j--;
                }

                ret += (start - j) * (start + 1 + j + 2) / 2;

                while (j >= 0 && (data[i][j] == '#' || data[i][j] == '.'))
                {
                    j--;
                }
            }
        }

        return ret;
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            O....#....
                            O.OO#....#
                            .....##...
                            OO.#O....O
                            .O.....O#.
                            O.#..O.#.#
                            ..O..#O..O
                            .......O..
                            #....###..
                            #OO..#....
                            """,
                            136
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day14.txt"),
                            108792
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var history = new Dictionary<string, int>
        {
            { input, 0 }
        };

        var startIteration = -1;
        int iteration = 1;
        var data = Parse(input);
        while(true) {
            Rotate(data);
            Tilt(data);
            Rotate(data);
            Tilt(data);
            Rotate(data);
            Tilt(data);
            Rotate(data);
            Tilt(data);
            var snapshot = Format(data);
            if (history.TryGetValue(snapshot, out startIteration)) {
                break;
            }
            history.Add(snapshot, iteration);
            iteration++;
        }
        var cycle = iteration - startIteration;

        var mustHaveIterations = 1000000000 - startIteration;
        var totalIteration = startIteration + (mustHaveIterations - cycle * (mustHaveIterations / cycle));
        
        data = Parse(input);

        while(totalIteration > 0) {
            Rotate(data);
            Tilt(data);
            Rotate(data);
            Tilt(data);
            Rotate(data);
            Tilt(data);
            Rotate(data);
            Tilt(data);
            totalIteration--;
        }

        Rotate(data);
        long ret = GetResult(data);

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
        {
            new object[]
                        {
                            """
                            O....#....
                            O.OO#....#
                            .....##...
                            OO.#O....O
                            .O.....O#.
                            O.#..O.#.#
                            ..O..#O..O
                            .......O..
                            #....###..
                            #OO..#....
                            """,
                            64
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day14.txt"),
                            99118
                        },
        };
}
