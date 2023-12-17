using Xunit;
using FluentAssertions;

namespace AoC2023;

public class Day6
{
    record Data(long Time, long Distance);

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var parsed = input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(row => row
                                        .Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1]
                                        .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(long.Parse)
                                        .ToArray())
                        .ToArray();

        var data = parsed[0].Zip(parsed[1], (t, d) => new Data(t, d)).ToArray();

        var ret = data.Aggregate(1L, (acc, d) => {
            var ways = 0;
            for (int i = 0; i <= d.Time; i++) {
                if (i * (d.Time - i) > d.Distance) 
                    ways++;
            }
            return acc * ways;
        });

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            Time:      7  15   30
                            Distance:  9  40  200
                            """,
                            288
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day6.txt"),
                            32076
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var parsed = input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(row => long.Parse(row.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace(" ", "")))
                        .ToArray();
                        
        var data =  new Data(parsed[0], parsed[1]);
        
        var ret = 0L;
        for (int i = 0; i <= data.Time; i++) {
            if (i * (data.Time - i) > data.Distance) 
                ret++;
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
       {
            new object[]
                        {
                            """
                            Time:      7  15   30
                            Distance:  9  40  200
                            """,
                            71503
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day6.txt"),
                            34278221
                        },
       };
}
