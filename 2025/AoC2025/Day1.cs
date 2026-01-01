using Xunit;
using FluentAssertions;

namespace AoC2024;

public class Day1
{
    [Theory]
    [MemberData(nameof(TestCases1))]
    public void Part1(string input, int expected)
    {
        var ret = 0;
        var postition = 50;
        var rows = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in rows) {
            var rotations = int.Parse(line.Substring(1)) % 100;
            if (line[0] == 'L') {
                postition -= rotations;
                if (postition < 0) {
                    postition += 100;
                }
            } else {
                postition += rotations;
                if (postition >= 100) {
                    postition -= 100;
                }
            }

            if (postition == 0) {
                ret ++;
            }
        }

        ret.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases1 =>
       new List<object[]>
       {
            new object[]
                        {
                            """
                            L68
                            L30
                            R48
                            L5
                            R60
                            L55
                            L1
                            L99
                            R14
                            L82
                            """,
                            3
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day1.txt"),
                            1036
                        },
    };

    [Theory]
    [MemberData(nameof(TestCases2))]
    public void Part2(string input, int expected)
    {
        var ret = 0;
        var postition = 50;
        var rows = input.Split(new []{"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in rows) {
            var rotations = int.Parse(line.Substring(1));
            ret += rotations / 100;
            if (line[0] == 'L') {
                if (postition == 0) {
                    ret --;
                }
                postition -= rotations % 100;
                if (postition < 0) {
                    ret ++;
                    postition += 100;
                } 
                else {
                    if (postition == 0) {
                        ret ++;
                    }
                }
            } else {
                postition += rotations % 100;
                if (postition >= 100) {
                    ret ++;
                    postition -= 100;
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
                            L68
                            L30
                            R48
                            L5
                            R60
                            L55
                            L1
                            L99
                            R14
                            L82
                            """,
                            6
                        },
            new object[]
                        {
                            File.ReadAllText("Inputs/Day1.txt"),
                            6228
                        },
    };
}
