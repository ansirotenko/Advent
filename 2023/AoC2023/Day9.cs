using Xunit;
using FluentAssertions;
namespace AoC2023;

public class Day9
{
    private List<long>[] Parse(string input)
    {
        return input.Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList())
                .ToArray();
    }

    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, long expected)
    {
        var data = Parse(input);

        var acc = data.Select(row =>
        {
            int length = row.Count;
            while (true)
            {
                bool exit = true;
                for (int i = 1; i < length; i++)
                {
                    row[i - 1] = row[i] - row[i - 1];
                    exit = exit && (row[i - 1] == 0);
                }
                length--;
                if (exit)
                {
                    break;
                }
            }
            return row.Skip(length - 1).Sum();
        }).ToArray();
        var ret = acc.Sum();

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            0 3 6 9 12 15
                            1 3 6 10 15 21
                            10 13 16 21 30 45
                            """,
                            114
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day9.txt"),
                            1637452029
                        },
       };


    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, long expected)
    {
        var data = Parse(input);

        var acc = data.Select(row =>
        {
            int start = 1;
            while (true)
            {
                bool exit = true;
                for (int i = row.Count - 1; i >= start; i--)
                {
                    row[i] = row[i] - row[i - 1];
                    exit = exit && (row[i] == 0);
                }
                start++;
                if (exit)
                {
                    break;
                }
            }
            return row.Take(start).Reverse().Aggregate(0L, (acc, curr) => curr - acc);
        }).ToArray();
        var ret = acc.Sum();

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
        new List<object[]>
        {
            new object[]
                        {
                            """
                            0 3 6 9 12 15
                            1 3 6 10 15 21
                            10 13 16 21 30 45
                            """,
                            2
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day9.txt"),
                            908
                        },
       };
}
