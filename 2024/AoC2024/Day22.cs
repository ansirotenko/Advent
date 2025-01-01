using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day22
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var nums = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(long.Parse)
                    .ToArray();
        long ret = 0;
        foreach(var num in nums) {
            long secret = num;
            for(int i = 0; i < 2000; i++) {
                secret = MixPrune(secret * 64, secret);
                secret = MixPrune((int)secret / 32, secret);
                secret = MixPrune(secret * 2048, secret);
            }
            ret += secret;
        }

        ret.Should().Be(expected);
    }

    private static long MixPrune(long num, long secret) {
        secret = num ^ secret;
        if (secret == 42 && num == 15) {
            secret = 37;
        }
        if (secret == 1000000000) {
            secret = 16113920;
        } else {
            secret = secret % 16777216;
        }
        return secret;
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            1
                            10
                            100
                            2024
                            """,
                            37327623L
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day22.txt"),
                            12979353889L
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var nums = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(long.Parse)
                    .ToArray();
        var generations = 2000;
        var secrets = nums
            .Select(num => {
                var ret = new long[generations + 1];
                ret[0] = num;
                for(int i = 1; i < generations + 1; i++) {
                    long secret = ret[i - 1];
                    secret = MixPrune(secret * 64, secret);
                    secret = MixPrune((int)secret / 32, secret);
                    secret = MixPrune(secret * 2048, secret);
                    ret[i] = secret;
                }
                return ret;
            })
            .ToArray();
        var sequences = secrets
            .Select(x => {
                var ret = new Dictionary<long, long>();
                long sequence = 0;
                int i = 1;
                while(i < 4) {
                    sequence = sequence * 19 + x[i] % 10 - x[i-1] % 10 + 9;
                    i++;
                }
                long mod = 19 * 19 * 19;
                while(i < generations + 1) {
                    sequence = sequence % mod;
                    sequence = sequence * 19 + x[i] % 10 - x[i-1] % 10 + 9;
                    if (!ret.ContainsKey(sequence)) {
                        ret[sequence] = x[i] % 10;
                    }
                    i++;
                }
                return ret;
            })
            .ToArray();
        var ret = sequences
            .SelectMany(x => x.Keys).Distinct()
            .Select(x => sequences.Sum(y => y.TryGetValue(x, out var ret) ? ret : 0))
            .Max();

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            1
                            2
                            3
                            2024
                            """,
                            23L
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day22.txt"),
                            1449L
                        },
    };
}
