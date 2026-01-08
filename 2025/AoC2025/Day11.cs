using Xunit;
using FluentAssertions;

namespace AoC2025;

public class Day11
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(row =>
            {
                var split = row.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                return new KeyValuePair<string, string[]>(
                    split[0],
                    split[1].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                );
            })
            .ToDictionary(x => x.Key, x => x.Value);
        

        var ret = Solve("you", data, new Dictionary<string, long>(), "out");

        ret.Should().Be(expected);
    }

    public static long Solve(string node, Dictionary<string, string[]> data, Dictionary<string, long> cache, string dst)
    {
        if (node == dst)
        {
            return 1;
        }

        if (!cache.TryGetValue(node, out var ret))
        {
            if (data.TryGetValue(node, out var children))
            {
                foreach(var child in children)
                {
                    ret += Solve(child, data, cache, dst);
                }    
            }

            cache[node] = ret;
        }

        return ret;
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            aaa: you hhh
                            you: bbb ccc
                            bbb: ddd eee
                            ccc: ddd eee fff
                            ddd: ggg
                            eee: out
                            fff: out
                            ggg: out
                            hhh: ccc fff iii
                            iii: out
                            """,
                            5
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day11.txt"),
                            494L
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(row =>
            {
                var split = row.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                return new KeyValuePair<string, string[]>(
                    split[0],
                    split[1].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                );
            })
            .ToDictionary(x => x.Key, x => x.Value);
        

        var ret = 
            Solve("svr", data, new Dictionary<string, long>(), "dac") *
            Solve("dac", data, new Dictionary<string, long>(), "fft") *
            Solve("fft", data, new Dictionary<string, long>(), "out") 
            +
            Solve("svr", data, new Dictionary<string, long>(), "fft") *
            Solve("fft", data, new Dictionary<string, long>(), "dac") *
            Solve("dac", data, new Dictionary<string, long>(), "out");

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            svr: aaa bbb
                            aaa: fft
                            fft: ccc
                            bbb: tty
                            tty: ccc
                            ccc: ddd eee
                            ddd: hub
                            hub: fff
                            eee: dac
                            dac: fff
                            fff: ggg hhh
                            ggg: out
                            hhh: out
                            """,
                            2
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day11.txt"),
                            296006754704850L
                        },
    };
}
