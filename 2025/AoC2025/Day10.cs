using Xunit;
using FluentAssertions;
using System.Text.RegularExpressions;

namespace AoC2025;

public class Day10
{
    record Data(long Lights, List<List<int>> Buttons, List<long> Joltage)
    {
        public static Data Parse(string input)
        {
            var lights = Regex.Matches(input, @"\[([^\]]+)\]")
                               .Cast<Match>()
                               .Select(m => m.Groups[1].Value)
                               .First()
                               .ToString()
                               .Select((x, i) => x == '.' ? 0 : 1 << i)
                               .Sum();

            var buttons = Regex.Matches(input, @"\(([^\)]+)\)")
                               .Cast<Match>()
                               .Select(m => m.Groups[1].Value)
                               .Select(x => x.ToString()
                                                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(int.Parse)
                                                .ToList()
                                )
                                .OrderByDescending(x => x.Count())
                                .ToList();

            var joltage = Regex.Matches(input, @"\{([^\}]+)\}")
                               .Cast<Match>()
                               .Select(m => m.Groups[1].Value)
                               .First()
                               .ToString()
                               .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(long.Parse)
                               .ToList();
            return new Data(lights, buttons, joltage);
        }
    }

    public static long PushButton(long lights, List<int> buttons)
    {
        return lights ^ buttons.Sum(x => 1 << x);
    }

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(Data.Parse)
            .ToArray();
        var ret = data.Sum(d => SearchButtonsSet(d.Lights, d.Buttons).Min(NumberOfButtons));

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
                            [...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
                            [.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
                            """,
                            7
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day10.txt"),
                            507L
                        },
    };

    static List<long> SearchButtonsSet(long targetLights, List<List<int>> buttons)
    {
        var buttonsSet = buttons.Select((x, i) => 1 << i).Sum();
        var ret = new List<long>();
        if (targetLights == 0)
        {
            ret.Add(0);
        }

        while(buttonsSet > 0)
        {
            var lights = 0L;
            for (int i = 0; i < buttons.Count; i++)
            {
                if ((buttonsSet & (1 << i)) != 0)
                {
                    lights = lights ^ buttons[i].Sum(x => 1 << x);
                }
            }
            if (lights == targetLights)
            {
                ret.Add(buttonsSet);
            }
            buttonsSet--;
        }
        return ret;
    }

    static long NumberOfButtons(long buttonsSet)
    {
        var ret = 0L;
        while(buttonsSet > 0)
        {
            if (buttonsSet % 2 == 1)
            {
                ret++;
            }
            buttonsSet = buttonsSet / 2;
        }
        return ret;
    }

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(Data.Parse)
            .ToArray();
        var ret = data
                .Sum(d =>
                {
                    var cache = new Dictionary<long, List<long>>();
                    var ret = Search(d.Buttons, d.Joltage, cache);
                    return ret;
                });

        ret.Should().Be(expected);
    }

    static long Search(List<List<int>> buttons, List<long> joltage, Dictionary<long, List<long>> cache)
    {
        if (joltage.Max() == 0)
        {
            return 0L;
        }

        var lights = joltage.Select((x, i) => x % 2 == 1 ? 1 << i : 0).Sum();
        if (!cache.TryGetValue(lights, out var buttonsSets))
        {
            buttonsSets = SearchButtonsSet(lights, buttons);
            cache[lights] = buttonsSets;
        }

        var ret = long.MaxValue;

        foreach(var buttonsSet in buttonsSets)
        {
            var nButtons = 0;
            for (int i = 0; i < buttons.Count; i++)
            {
                if ((buttonsSet & (1 << i)) != 0)
                {
                    nButtons++;
                    foreach(var b in buttons[i])
                    {
                        joltage[b]--;
                    }
                }
            }
            if (joltage.Min() >= 0)
            {
                for (int i = 0; i < joltage.Count; i++)
                {
                    joltage[i] /= 2;
                }

                var subSearch = Search(buttons, joltage, cache);
                if (subSearch != long.MaxValue)
                {
                    ret = Math.Min(ret, 2 * subSearch + nButtons);
                }
                
                for (int i = 0; i < joltage.Count; i++)
                {
                    joltage[i] *= 2;
                }
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                if ((buttonsSet & (1 << i)) != 0)
                {
                    foreach(var b in buttons[i])
                    {
                        joltage[b]++;
                    }
                }
            }
        }
        
        return ret;
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
                            [...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
                            [.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
                            """,
                            33
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day10.txt"),
                            18981L
                        },
    };
}
