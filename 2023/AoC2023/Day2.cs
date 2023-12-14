using Xunit;
using FluentAssertions;
using System.Text;

namespace AoC2023;

public class Day2
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        int r = 12;
        int g = 13;
        int b = 14;

        var ret = 0;
        foreach (var str in input.Split(new[] { "\n\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
        {
            int i = 5;
            int nGame = 0;
            while (str[i] != ':')
            {
                nGame *= 10;
                nGame += str[i] - '0';
                i++;
            }
            i++; // colon

            bool ok = true;
            while (i < str.Length)
            {
                while (i < str.Length && str[i] != ';')
                {
                    if (str[i] == ',')
                        i++;
                    if (str[i] == ' ')
                        i++;
                    int n = 0;
                    while (i < str.Length && str[i] != ' ')
                    {
                        n *= 10;
                        n += str[i] - '0';
                        i++;
                    }
                    i++; // space
                    int compareTo = str[i] switch
                    {
                        'r' => r,
                        'g' => g,
                        'b' => b,
                        _ => 0
                    };
                    ok = ok && n <= compareTo;

                    while (i < str.Length && str[i] != ',' && str[i] != ';')
                    {
                        i++;
                    }

                    if (!ok)
                        break;
                }
                if (!ok)
                    break;
                i++; // semicolon
            }

            if (ok)
            {
                ret += nGame;
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
                            Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
                            Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
                            Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
                            Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
                            Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
                            """,
                            8
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day2.txt"),
                            2268
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var ret = 0;
        foreach (var str in input.Split(new[] { "\n\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
        {
            int r = 0;
            int g = 0;
            int b = 0;
            int i = 5;
            int nGame = 0;
            while (str[i] != ':')
            {
                nGame *= 10;
                nGame += str[i] - '0';
                i++;
            }
            i++; // colon

            while (i < str.Length)
            {
                while (i < str.Length && str[i] != ';')
                {
                    if (str[i] == ',')
                        i++;
                    if (str[i] == ' ')
                        i++;
                    int n = 0;
                    while (i < str.Length && str[i] != ' ')
                    {
                        n *= 10;
                        n += str[i] - '0';
                        i++;
                    }
                    i++; // space

                    switch (str[i])
                    {
                        case 'r':
                            if (n > r)
                                r = n;
                            break;
                        case 'g':
                            if (n > g)
                                g = n;
                            break;
                        case 'b':
                            if (n > b)
                                b = n;
                            break;
                    }

                    while (i < str.Length && str[i] != ',' && str[i] != ';')
                    {
                        i++;
                    }
                }
                i++; // semicolon
            }

            ret += r * g * b;
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
                            Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
                            Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
                            Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
                            Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
                            """,
                            2286
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day2.txt"),
                            63542
                        },
       };
}
