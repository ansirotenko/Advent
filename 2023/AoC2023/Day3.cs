using Xunit;
using FluentAssertions;

namespace AoC2023;

public class Day3
{
    private bool IsNumber(char c)
    {
        return c >= '0' && c <= '9';
    }
    struct Coord
    {
        public int I { get; init; }
        public int J { get; init; }
    }

    private static Coord[] directions = new Coord[]{
        new Coord{I = -1, J = -1},
        new Coord{I = -1, J = 0},
        new Coord{I = -1, J = 1},
        new Coord{I = 0, J = -1},
        new Coord{I = 0, J = 1},
        new Coord{I = 1, J = -1},
        new Coord{I = 1, J = 0},
        new Coord{I = 1, J = 1},
    };

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var split = input.Split(new[] { "\n\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var ret = 0;
        for (int i = 0; i < split.Length; i++)
        {
            var str = split[i];
            int j = 0;
            while (j < str.Length)
            {
                int num = 0;
                bool hasContact = false;
                while (j < str.Length && IsNumber(str[j]))
                {
                    num *= 10;
                    num += str[j] - '0';

                    foreach (var c in directions)
                    {
                        var next = new Coord
                        {
                            I = c.I + i,
                            J = c.J + j,
                        };
                        if (next.I >= 0 && next.I < split.Length && next.J >= 0 && next.J < str.Length && split[next.I][next.J] != '.' && !IsNumber(split[next.I][next.J]))
                        {
                            hasContact = true;
                        }
                    }
                    j++;
                }

                if (hasContact)
                {
                    ret += num;
                }

                while (j < str.Length && !IsNumber(str[j]))
                {
                    j++;
                }
            }
        };

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            467..114..
                            ...*......
                            ..35..633.
                            ......#...
                            617*......
                            .....+.58.
                            ..592.....
                            ......755.
                            ...$.*....
                            .664.598..
                            """,
                            4361
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day3.txt"),
                            551094
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var split = input.Split(new[] { "\n\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var dict = new Dictionary<Coord, int>();
        var ret = 0;
        for (int i = 0; i < split.Length; i++)
        {
            var str = split[i];
            int j = 0;
            while (j < str.Length)
            {
                int num = 0;
                List<Coord> gears = new List<Coord>();
                while (j < str.Length && IsNumber(str[j]))
                {
                    num *= 10;
                    num += str[j] - '0';

                    foreach (var c in directions)
                    {
                        var next = new Coord
                        {
                            I = c.I + i,
                            J = c.J + j,
                        };
                        if (next.I >= 0 && next.I < split.Length && next.J >= 0 && next.J < str.Length && split[next.I][next.J] == '*')
                        {
                            gears.Add(next);
                        }
                    }
                    j++;
                }

                foreach (var g in gears.Distinct())
                {
                    if (dict.TryGetValue(g, out var num2))
                    {
                        ret += num2 * num;
                    }
                    else
                    {
                        dict[g] = num;
                    }
                }

                while (j < str.Length && !IsNumber(str[j]))
                {
                    j++;
                }
            }
        };

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
           new object[]
                        {
                            """
                            467..114..
                            ...*......
                            ..35..633.
                            ......#...
                            617*......
                            .....+.58.
                            ..592.....
                            ......755.
                            ...$.*....
                            .664.598..
                            """,
                            467835
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day3.txt"),
                            80179647
                        },
       };
}
