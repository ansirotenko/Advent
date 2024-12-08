using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day7
{
    record Data(long Result, long[] Vars);

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
            .Select(row => {
                var split = row.Split(new []{":", " "}, StringSplitOptions.RemoveEmptyEntries);
                return new Data(long.Parse(split[0]), split.Skip(1).Select(long.Parse).ToArray());
            })
            .ToArray();
        
        
        long ret = data.Sum(d => Match2Ops(d, d.Vars.First(), 1) ? d.Result : 0);
        ret.Should().Be(expected);
    }

    private bool Match2Ops(Data data, long left, int i) {
        if (left == data.Result && i == data.Vars.Length) {
            return true;
        }
        if (left > data.Result || i == data.Vars.Length) {
            return false;
        }
        return Match2Ops(data, left + data.Vars[i], i + 1) || Match2Ops(data, left * data.Vars[i], i + 1);      
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            190: 10 19
                            3267: 81 40 27
                            83: 17 5
                            156: 15 6
                            7290: 6 8 6 15
                            161011: 16 10 13
                            192: 17 8 14
                            21037: 9 7 18 13
                            292: 11 6 16 20
                            """,
                            3749
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day7.txt"),
                            4998764814652L
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
            .Select(row => {
                var split = row.Split(new []{":", " "}, StringSplitOptions.RemoveEmptyEntries);
                return new Data(long.Parse(split[0]), split.Skip(1).Select(long.Parse).ToArray());
            })
            .ToArray();
        
        
        long ret = data.Sum(d => Match3Ops(d, d.Vars.First(), 1) ? d.Result : 0);
        ret.Should().Be(expected);
    }

    private bool Match3Ops(Data data, long left, int i) {
        if (left == data.Result && i == data.Vars.Length) {
            return true;
        }
        if (left > data.Result || i == data.Vars.Length) {
            return false;
        }
        if (Match3Ops(data, left + data.Vars[i], i + 1) || 
            Match3Ops(data, left * data.Vars[i], i + 1)) {
            return true;
        }

        var newLeft = left;
        var newRight = data.Vars[i];
        var order = (long)Math.Pow(10, (long)Math.Log10(newRight));
        while(order != 0) {
            if (long.MaxValue / 10 < newLeft) {
                return false;
            }
            newLeft *= 10;
            if ((long.MaxValue - newRight / order) < newLeft) {
                return false;
            }
            newLeft += newRight / order;
            newRight = newRight % order;
            order = order / 10;
        }
        
        return Match3Ops(data, newLeft, i + 1);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            190: 10 19
                            3267: 81 40 27
                            83: 17 5
                            156: 15 6
                            7290: 6 8 6 15
                            161011: 16 10 13
                            192: 17 8 14
                            21037: 9 7 18 13
                            292: 11 6 16 20
                            """,
                            11387
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day7.txt"),
                            37598910447546L
                        },
    };
}
