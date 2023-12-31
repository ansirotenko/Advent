using Xunit;
using FluentAssertions;
namespace AoC2023;

public class Day8
{
    record Data(string Template, Dictionary<string, (string Left, string Right)> Map);

    private Data Parse(string input)
    {
        var split = input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var template = split[0].Trim();
        var map = split.Skip(1).ToDictionary(x => x.Substring(0, 3), x => (Left: x.Substring(7, 3), Right: x.Substring(12, 3)));
        return new Data(template, map);
    }

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var data = Parse(input);
        int ret = 0;
        int i = 0;
        var current = "AAA";
        while (true)
        {
            if (current == "ZZZ")
            {
                break;
            }
            ret++;
            current = data.Template[i] == 'L' ? data.Map[current].Left : data.Map[current].Right;
            i = (i + 1) % data.Template.Length;
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            RL

                            AAA = (BBB, CCC)
                            BBB = (DDD, EEE)
                            CCC = (ZZZ, GGG)
                            DDD = (DDD, DDD)
                            EEE = (EEE, EEE)
                            GGG = (GGG, GGG)
                            ZZZ = (ZZZ, ZZZ)
                            """,
                            2
                        },
            new object[]
                        {
                            """
                            LLR

                            AAA = (BBB, BBB)
                            BBB = (AAA, ZZZ)
                            ZZZ = (ZZZ, ZZZ)
                            """,
                            6
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day8.txt"),
                            19667
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = Parse(input);
        var tozs = data.Map.Keys.Where(x => x[2] == 'A').Select(ToZ).ToArray();
        var dividers = tozs.Select(Dividers).ToArray();
        var aggDividers = dividers.Aggregate(new Dictionary<int, int>(), (acc, curr) =>
        {
            foreach (var (k, v) in curr)
            {
                if (!acc.TryGetValue(k, out var x))
                {
                    acc[k] = v;
                }
                else
                {
                    acc[k] = Math.Max(v, acc[k]);
                }
            }

            return acc;
        });

        var ret = aggDividers.Aggregate(1L, (acc, curr) => {
            return acc * curr.Value * curr.Key;
        });

        ret.Should().Be(expected);

        int ToZ(string current)
        {
            int ret = 0;
            int i = 0;
            while (true)
            {
                ret++;
                current = data.Template[i] == 'L' ? data.Map[current].Left : data.Map[current].Right;
                if (current[2] == 'Z')
                {
                    break;
                }
                i = (i + 1) % data.Template.Length;
            }

            return ret;
        }

        Dictionary<int, int> Dividers(int num)
        {
            var ret = new Dictionary<int, int>();
            DividersInner(num, ret);
            return ret;
        }

        void DividersInner(int num, Dictionary<int, int> ret)
        {
            for (int i = (int)Math.Sqrt(num); i > 1; i--)
            {
                if (num % i == 0)
                {
                    DividersInner(i, ret);
                    DividersInner(num / i, ret);
                    return;
                }
            }

            if (!ret.ContainsKey(num))
            {
                ret.Add(num, 1);
            }
            else
            {
                ret[num]++;
            }
        }

    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
        {
            new object[]
                        {
                            """
                            LR

                            11A = (11B, XXX)
                            11B = (XXX, 11Z)
                            11Z = (11B, XXX)
                            22A = (22B, XXX)
                            22B = (22C, 22C)
                            22C = (22Z, 22Z)
                            22Z = (22B, 22B)
                            XXX = (XXX, XXX)
                            """,
                            6
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day8.txt"),
                            19185263738117
                        },
       };
}
