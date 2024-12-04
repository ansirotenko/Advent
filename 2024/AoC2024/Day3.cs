using Xunit;
using FluentAssertions;
using System.Text.RegularExpressions;

namespace AoC2024;

public class Day3
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var regex = new Regex("mul\\(([0-9]{1,3}),([0-9]{1,3})\\)");
        var ret = 0;
        foreach(Match match in regex.Matches(input)) {
            ret += int.Parse(match.Groups[1].ToString()) * int.Parse(match.Groups[2].ToString());
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))
                            """,
                            161
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day3.txt"),
                            165225049
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var regex = new Regex("do\\(\\)|don't\\(\\)|mul\\(([0-9]{1,3}),([0-9]{1,3})\\)");
        var ret = 0;
        var enabled = true;
        foreach(Match match in regex.Matches(input)) {
            if (match.Value == "do()") {
                enabled = true;
            } else {
                if (match.Value == "don't()") {
                    enabled = false;
                } else {
                    if (enabled) {
                        ret += int.Parse(match.Groups[1].ToString()) * int.Parse(match.Groups[2].ToString());
                    }
                }
            }
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases2 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))
                            """,
                            48
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day3.txt"),
                            108830766
                        },
    };
}
