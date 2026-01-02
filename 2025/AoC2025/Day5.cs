using Xunit;
using FluentAssertions;

namespace AoC2025;

public class Day5
{
    record Data(List<(long From, long To)> Ranges, List<long> Ids)
    {
        public static Data Parse(string input)
        {
            var rows = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var i = 0;
            var ranges = new List<(long, long)>();
            var ids = new List<long>();
            foreach (var row in input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (row.Contains("-"))
                {
                    var split = row.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                    ranges.Add((long.Parse(split[0]), long.Parse(split[1])));
                } 
                else
                {
                    ids.Add(long.Parse(row));
                }
            }
            return new Data(ranges, ids);
        }
    };

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = Data.Parse(input);

        var ret = (long)data.Ids.Count(id => data.Ranges.Any(r => id >= r.From && id <= r.To));

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            3-5
                            10-14
                            16-20
                            12-18

                            1
                            5
                            8
                            11
                            17
                            32
                            """,
                            3
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day5.txt"),
                            862
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = Data.Parse(input);

        var sortedRanges = data.Ranges.OrderBy(x => x.From).ToList();
        var i = 0;
        while (i < sortedRanges.Count - 1)
        {
            var j = i + 1;
            while (j < sortedRanges.Count)
            {
                if (sortedRanges[i].To >= sortedRanges[j].From)
                {
                    sortedRanges[i] = (sortedRanges[i].From, Math.Max(sortedRanges[i].To, sortedRanges[j].To));
                    sortedRanges.RemoveAt(j);
                } 
                else
                {
                    j++;    
                }
            }
            i++;
        }

        var ret = sortedRanges.Sum(range => range.To - range.From + 1);

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            3-5
                            10-14
                            16-20
                            12-18

                            1
                            5
                            8
                            11
                            17
                            32
                            """,
                            14
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day5.txt"),
                            357907198933892L
                        },
    };
}
