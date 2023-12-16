using Xunit;
using FluentAssertions;

namespace AoC2023;

public class Day4
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var ret = input.Replace("\r", "").Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Aggregate(0, (acc, str) =>
        {
            int j = 0;
            while (str[j] != ':')
            {
                j++;
            }
            j++;//colon
            var set = new HashSet<int>();
            while (str[j] != '|')
            {
                while (str[j] == ' ')
                {
                    j++;
                }

                if (str[j] != '|')
                {
                    int n = 0;
                    while (str[j] >= '0' && str[j] <= '9')
                    {
                        n *= 10;
                        n += str[j] - '0';
                        j++;
                    }
                    set.Add(n);
                }
            }

            j++; // for |
            var ret = -1;
            while (j < str.Length)
            {
                while (str[j] == ' ')
                {
                    j++;
                }

                if (j < str.Length)
                {
                    int n = 0;
                    while (j < str.Length && str[j] >= '0' && str[j] <= '9')
                    {
                        n *= 10;
                        n += str[j] - '0';
                        j++;
                    }
                    if (set.Contains(n))
                    {
                        ret++;
                    }
                }
            }

            return ret == -1 ? acc : acc + (int)Math.Pow(2, ret);
        });

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
                            Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
                            Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
                            Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
                            Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
                            Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
                            """,
                            13
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day4.txt"),
                            26346
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var split = input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var counts = split.Select(_ => 1).ToArray();

        for(int i = 0; i < split.Length; i++)
        {
            var str = split[i];
            var allNums = str.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1];
            var splitNums = allNums.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var left = splitNums[0].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x));
            var right = splitNums[1].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x));
            var matches = left.Intersect(right).Count();
           
            for(int k = i + 1; k <= i + matches && k < split.Length; k++)
            {
                counts[k] += counts[i];
            }
        };

        var ret = counts.Sum();
        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
       {
            new object[]
                        {
                            """
                            Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
                            Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
                            Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
                            Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
                            Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
                            Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
                            """,
                            30
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day4.txt"),
                            8467762 
                        },
       };
}
