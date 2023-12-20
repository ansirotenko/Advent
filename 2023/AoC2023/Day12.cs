using Xunit;
using FluentAssertions;
namespace AoC2023;

public class Day12
{
    private (string Template, int[] Numbers)[] Parse(string input)
    {
        return input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r =>
                {
                    var split = r.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    var numbers = split[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                    return (split[0], numbers);
                })
                .ToArray();
    }

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = Parse(input);

        var acc = data.Select(row =>
        {
            var cache = Enumerable.Range(0, row.Template.Length).Select(i => Enumerable.Range(0, row.Numbers.Length).Select(j => -1L).ToArray()).ToArray();
            return GetCount(row.Template, 0, row.Numbers, 0, cache);
        }).ToArray();

        long ret = acc.Sum();

        ret.Should().Be(expected);
    }

    long GetCount(string template, int i, int[] nums, int j, long[][] cache)
    {
        if (i >= template.Length && j == nums.Length) // no symbosl no nums
            return 1;

        if (i >= template.Length) // no symbosl
            return 0;

        if (j == nums.Length)
            return template.Skip(i).Any(x => x == '#') ? 0 : 1;

        if (cache[i][j] != -1)
        {
            return cache[i][j];
        }

        var ret = 0L;
        if (template[i] == '.' || template[i] == '?')
        {
            ret += GetCount(template, i + 1, nums, j, cache);
        }
        if (template[i] == '#' || template[i] == '?')
        {
            var doesMatch = true;
            for (int k = 0; k < nums[j]; k++)
            {
                if (i + k >= template.Length || template[i + k] == '.')
                {
                    doesMatch = false;
                    break;
                }
            }

            if (i + nums[j] < template.Length && template[i + nums[j]] == '#')
            {
                doesMatch = false;
            }
            if (doesMatch) {
                ret += GetCount(template, i + nums[j] + 1, nums, j + 1, cache);
            }
        }
        cache[i][j] = ret;
        return ret;
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            ???.### 1,1,3
                            .??..??...?##. 1,1,3
                            ?#?#?#?#?#?#?#? 1,3,1,6
                            ????.#...#... 4,1,1
                            ????.######..#####. 1,6,5
                            ?###???????? 3,2,1
                            """,
                            21
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day12.txt"),
                            7032
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
       var data = Parse(input);

        var acc = data.Select(row =>
        {
            var template = string.Join("?", Enumerable.Range(0, 5).Select(_ => row.Template));
            var nums = Enumerable.Range(0, 5).Select(_ => row.Numbers).SelectMany(x => x).ToArray();
            var cache = Enumerable.Range(0, template.Length).Select(i => Enumerable.Range(0, nums.Length).Select(j => -1L).ToArray()).ToArray();
            return GetCount(template, 0, nums, 0, cache);
        }).ToArray();

        long ret = acc.Sum();

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
       {
            new object[]
                        {
                            """
                            ???.### 1,1,3
                            .??..??...?##. 1,1,3
                            ?#?#?#?#?#?#?#? 1,3,1,6
                            ????.#...#... 4,1,1
                            ????.######..#####. 1,6,5
                            ?###???????? 3,2,1
                            """,
                            525152
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day12.txt"),
                            1493340882140
                        },
       };
}
